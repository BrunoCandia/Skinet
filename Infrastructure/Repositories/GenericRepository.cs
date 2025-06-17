using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.SpecificationEvaluators;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _storeContext;

        public GenericRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _storeContext.Set<T>().AddAsync(entity, cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var query = _storeContext.Set<T>().AsQueryable();

            query = spec.ApplyCriteria(query);

            return await query.CountAsync(cancellationToken);
        }

        public void Delete(T entity)
        {
            _storeContext.Set<T>().Remove(entity);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _storeContext.Set<T>().AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _storeContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _storeContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetEntitiesWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TResult>> GetEntitiesWithSpecAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification<TResult>(spec).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a single entity that matches the specified criteria.
        /// </summary>
        /// <remarks>This method applies the given specification to filter (without projections) and retrieves the
        /// first matching result. Use this method when you expect at most one entity to match the criteria.</remarks>
        /// <param name="spec">The specification defining the criteria for filtering entities.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The first entity that matches the criteria defined in the <paramref name="spec"/>,  or <see
        /// langword="null"/> if no matching entity is found.</returns>
        public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a single entity that matches the specified criteria.
        /// </summary>
        /// <remarks>This method applies the given specification to filter and project entities, returning
        /// the first result that matches the criteria. If no entities match, the method returns <see
        /// langword="null"/>.</remarks>
        /// <typeparam name="TResult">The type of the result produced by the specification.</typeparam>
        /// <param name="spec">The specification defining the criteria for filtering and projecting the entity.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The first entity that matches the criteria defined in the specification, or <see langword="null"/> if no
        /// matching entity is found.</returns>
        public async Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification<TResult>(spec).FirstOrDefaultAsync(cancellationToken);
        }

        ////public async Task<bool> SaveChangesAsync()
        ////{
        ////    return await _storeContext.SaveChangesAsync() > 0;
        ////}

        public void Update(T entity)
        {
            ////_storeContext.Set<T>().Attach(entity);
            ////_storeContext.Entry(entity).State = EntityState.Modified;

            _storeContext.Set<T>().Update(entity);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_storeContext.Set<T>().AsQueryable(), spec);
        }

        private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> spec)
        {
            return SpecificationEvaluator<T>.GetQuery<T, TResult>(_storeContext.Set<T>().AsQueryable(), spec);
        }
    }
}
