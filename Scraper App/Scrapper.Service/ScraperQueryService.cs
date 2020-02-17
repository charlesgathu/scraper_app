using Microsoft.Extensions.Logging;
using Scraper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapper.Service
{
    public class ScraperQueryService : IScraperQueryService
    {

        private readonly IShowRepository repository;
        private readonly ILogger<ScraperQueryService> logger;

        public ScraperQueryService(IShowRepository repository, ILogger<ScraperQueryService> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(IShowRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(IShowRepository));
        }

        public async Task<IEnumerable<ShowResponseDto>> GetShowsAsync(int index)
        {
            var shows = await repository.GetShowsAsync(index);
            logger.LogInformation($"Queried shows for {index} index");
            var showResponses = shows.Select(sel => new ShowResponseDto
            {
                Id = sel.Id,
                Name = sel.Name,
                Cast = sel.ShowCast.Select(se => new CastResposeDto
                {
                    Id = se.Cast.Id,
                    Name = se.Cast.Name,
                    Birthday = se.Cast.Birthday?.ToString("yyyy-MM-dd")
                }).ToArray()
            }).ToArray();
            return showResponses;
        }

    }
}
