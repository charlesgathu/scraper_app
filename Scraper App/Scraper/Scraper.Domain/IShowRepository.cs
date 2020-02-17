using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper.Domain
{
    public interface IShowRepository
    {

        IUnitOfWork UnitOfWork { get; }

        Show Add(Show show);

        void Update(Show show);

        Task<Show> GetAsync(int showId);

        Task<IEnumerable<Show>> GetShowsAsync(int index);

        Task<int> GetLastIndexAsync();

        Task<Cast> GetCastAsync(int castId);

        Task<ShowCast> GetShowCastAsync(int showId, int castId);
       
    }
}
