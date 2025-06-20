﻿using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System.Collections.Concurrent;
using System.Threading;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _storeContext;
        private readonly ConcurrentDictionary<string, object> _repositories = new();

        public UnitOfWork(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task<bool> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _storeContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public void Dispose()
        {
            _storeContext.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity).Name, valueFactory =>
            {
                var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
                return Activator.CreateInstance(repositoryType, _storeContext) ?? throw new InvalidOperationException($"Could not create repository for type {typeof(TEntity).Name}");
            });
        }
    }
}
