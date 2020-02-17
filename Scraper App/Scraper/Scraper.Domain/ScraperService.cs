using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Domain
{
    public class ScraperService : IScraperService
    {

        private readonly IShowRepository repository;

        public ScraperService(IShowRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(IShowRepository));
        }

        // function to import shows
        public async Task AddShowAsync(Show show)
        {
            await AddShowItemAsync(show);
            await repository.UnitOfWork.SaveEntitiesAsync();
        }

        private async Task AddShowItemAsync(Show show)
        {
            var myShow = await repository.GetAsync(show.Id);
            if (myShow != null) return;

            repository.Add(show);
        }

        public async Task AddShows(IEnumerable<Show> shows)
        {
            foreach (var showItem in shows)
            {
                await AddShowItemAsync(showItem);
            }
            await repository.UnitOfWork.SaveEntitiesAsync();
        }

        public Task<IEnumerable<Show>> GetShows(int index)
        {
            return repository.GetShowsAsync(index);
        }

    }
}
