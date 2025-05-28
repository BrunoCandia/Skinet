using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SpecificationEvaluators
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        protected SpecificationEvaluator() { }

        public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec)
        {
            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria); // x => x.Brand == brand
            }

            if (spec.OrderBy is not null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescending is not null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsDistinct)
            {
                query = query.Distinct();
            }

            if (spec.IsPagingEndabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            // for eager loading
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = spec.IncludesStrings.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }

        public static IQueryable<TResult> GetQuery<TSpec, TResult>(IQueryable<T> query, ISpecification<T, TResult> spec)
        {
            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria); // x => x.Brand == brand
            }

            if (spec.OrderBy is not null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescending is not null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            var selectQuery = query as IQueryable<TResult>;

            if (spec.Select is not null)
            {
                selectQuery = query.Select(spec.Select);
            }

            if (spec.IsDistinct)
            {
                selectQuery = selectQuery?.Distinct();
            }

            if (spec.IsPagingEndabled)
            {
                selectQuery = selectQuery?.Skip(spec.Skip).Take(spec.Take);
            }

            return selectQuery ?? query.Cast<TResult>();
        }
    }
}
