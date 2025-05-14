using Core.Entities;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> SaveChangesAsync();
        Task<bool> ExistsAsync(Guid id);

        Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);
        Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec);
        Task<IReadOnlyList<T>> GetEntitiesWithSpecAsync(ISpecification<T> spec);
        Task<IReadOnlyList<TResult>> GetEntitiesWithSpecAsync<TResult>(ISpecification<T, TResult> spec);
    }
}
