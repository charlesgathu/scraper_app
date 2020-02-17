using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Domain
{
    public interface IUnitOfWork
    {

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);

    }
}
