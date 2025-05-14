using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.SpecificationEvaluators;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _storeContext;

        public GenericRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task AddAsync(T entity)
        {
            await _storeContext.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _storeContext.Set<T>().Remove(entity);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _storeContext.Set<T>().AnyAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _storeContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _storeContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetEntitiesWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<IReadOnlyList<TResult>> GetEntitiesWithSpecAsync<TResult>(ISpecification<T, TResult> spec)
        {
            return await ApplySpecification<TResult>(spec).ToListAsync();
        }

        public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec)
        {
            return await ApplySpecification<TResult>(spec).FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _storeContext.SaveChangesAsync() > 0;
        }

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
