using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper.Domain
{
    public interface IScraperService
    {
        Task AddShowAsync(Show show);

        Task AddShows(IEnumerable<Show> shows);

        Task<IEnumerable<Show>> GetShows(int index);

    }
}