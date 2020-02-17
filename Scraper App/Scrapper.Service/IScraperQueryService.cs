using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapper.Service
{
    public interface IScraperQueryService
    {
        Task<IEnumerable<ShowResponseDto>> GetShowsAsync(int index);
    }
}