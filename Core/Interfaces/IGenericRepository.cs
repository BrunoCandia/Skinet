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
        ////Task<bool> SaveChangesAsync();
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Retrieves a single entity that matches the specified criteria.
        /// </summary>
        /// <remarks>This method applies the given specification to filter (without projections) and retrieves the
        /// first matching result. Use this method when you expect at most one entity to match the criteria.</remarks>
        /// <param name="spec">The specification defining the criteria for filtering entities.</param>
        /// <returns>The first entity that matches the criteria defined in the <paramref name="spec"/>,  or <see
        /// langword="null"/> if no matching entity is found.</returns>
        Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);

        /// <summary>
        /// Retrieves a single entity that matches the specified criteria.
        /// </summary>
        /// <remarks>This method applies the given specification to filter and project entities, returning
        /// the first result that matches the criteria. If no entities match, the method returns <see
        /// langword="null"/>.</remarks>
        /// <typeparam name="TResult">The type of the result produced by the specification.</typeparam>
        /// <param name="spec">The specification defining the criteria for filtering and projecting the entity.</param>
        /// <returns>The first entity that matches the criteria defined in the specification, or <see langword="null"/> if no
        /// matching entity is found.</returns>
        Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec);
        Task<IReadOnlyList<T>> GetEntitiesWithSpecAsync(ISpecification<T> spec);
        Task<IReadOnlyList<TResult>> GetEntitiesWithSpecAsync<TResult>(ISpecification<T, TResult> spec);
        Task<int> CountAsync(ISpecification<T> spec);
    }
}
