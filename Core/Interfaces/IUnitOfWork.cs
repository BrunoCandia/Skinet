using Core.Entities;
using System.Threading;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<bool> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
