using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scraper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Scrapper.Service
{
    public class ScrapperService : IHostedService
    {

        private readonly string serviceUrl;
        private readonly IShowRepository showRepository;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly ILogger<ScrapperService> logger;

        public ScrapperService(string serviceUrl, IShowRepository showRepository, IHostApplicationLifetime applicationLifetime, ILogger<ScrapperService> logger)
        {
            this.serviceUrl = serviceUrl ?? throw new ArgumentNullException(nameof(serviceUrl));
            this.showRepository = showRepository ?? throw new ArgumentNullException(nameof(IShowRepository));
            this.applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(IHostApplicationLifetime));
            this.logger = logger ?? throw new ArgumentNullException(nameof(ILogger<ScrapperService>));
        }

        public async Task QueryShowsAsync()
        {
            bool canQuery = true;
            int index = await showRepository.GetLastIndexAsync();

            logger.LogInformation($"Scraping from index {index}");
            while (canQuery)
            {
                var client = new HttpClient();
                var response = await client.GetAsync($"{serviceUrl}shows?page={index}");
                if (!response.IsSuccessStatusCode)
                {
                    // handle error responses
                    switch ((int)response.StatusCode)
                    {
                        case (int)System.Net.HttpStatusCode.NotFound:
                            // reached end of process so terminate
                            // better option would be to wait for like a day before checking
                            canQuery = false;

                            logger.LogInformation($"Gotten to the end of the list at index {index}");
                            return;
                        case 429: // rate limited so wait for a minute before retrying
                        default: // unexpected error. Wait for a minute before retrying as well
                            logger.LogInformation($"Ratelimit experienced at index {index}");
                            await Task.Delay(TimeSpan.FromMinutes(1));
                            continue;
                    }
                }
                var content = await response.Content.ReadAsStringAsync();
                var shows = JsonConvert.DeserializeObject<List<ShowDto>>(content);
                foreach (var show in shows)
                {
                    try
                    {
                        var cast = await GetCastAsync(show.Id);

                        var showItem = await showRepository.GetAsync(show.Id);
                        if (showItem != null) continue;

                        _ = showRepository.Add(new Show
                        {
                            Id = show.Id,
                            Name = show.Name,
                            ShowCast = cast.GroupBy(gp => gp.Person.Id).Select(sel =>
                           {

                               var castItem = showRepository.GetCastAsync(sel.First().Person.Id).Result;
                               if (castItem == null)
                               {
                                   castItem = new Cast
                                   {
                                       Id = sel.First().Person.Id,
                                       Name = sel.First().Person.Name,
                                       Birthday = string.IsNullOrEmpty(sel.First().Person.Birthday) ? new DateTime?() : DateTime.ParseExact(sel.First().Person.Birthday, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None)
                                   };
                               }

                               var showCast = showRepository.GetShowCastAsync(show.Id, sel.First().Person.Id).Result;
                               if (showCast == null)
                               {
                                   showCast = new ShowCast
                                   {
                                       ShowId = show.Id,
                                       Cast = castItem,
                                       CastId = castItem.Id
                                   };
                               }
                               return showCast;
                           }).ToArray()
                        });
                        await showRepository.UnitOfWork.SaveEntitiesAsync();
                        logger.LogInformation($"Successfully saved the show '{show.Name}' with id: {show.Id}");
                    }
                    catch (Exception excp)
                    {
                        logger.LogError(excp, $"Unexpected error encountered while persisting show {show.Id}");
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }
                }
                index++;
                logger.LogInformation($"Incrementing the index to {index}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service Start initiated...");
            applicationLifetime.ApplicationStarted.Register(OnStarting);
            applicationLifetime.ApplicationStopping.Register(OnStopping);
            return Task.CompletedTask;
        }

        private void OnStarting()
        {
            logger.LogInformation("Service Starting...");
            var task = Task.Run(async () =>
            {
                await QueryShowsAsync();
            });
        }

        private void OnStopping()
        {
            logger.LogInformation("Service stopping...");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service Stop initiated...");
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<CastDto>> GetCastAsync(int showId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{serviceUrl}shows/{showId}/cast");
            if (!response.IsSuccessStatusCode)
            {
                // handle error responses
                switch ((int)response.StatusCode)
                {
                    case 429: // rate limited so wait for a minute before retrying
                        throw new RateLimitException($"Unable to retrieve cast for showId: {showId}");
                    default: // unexpected error. Wait for a minute before retrying as well
                        throw new Exception("Unable to retrieve cast");
                }
            }
            var content = await response.Content.ReadAsStringAsync();
            var cast = JsonConvert.DeserializeObject<List<CastDto>>(content);
            return cast;
        }

    }
}
