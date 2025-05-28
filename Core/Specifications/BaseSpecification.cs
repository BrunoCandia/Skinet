using Core.Interfaces;
using System.Linq.Expressions;

namespace Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        private readonly Expression<Func<T, bool>>? _criteria;

        protected BaseSpecification() : this(null) { }

        public BaseSpecification(Expression<Func<T, bool>>? criteria)
        {
            _criteria = criteria;
        }

        // Expression body for property
        public Expression<Func<T, bool>>? Criteria => _criteria;

        ////public Expression<Func<T, bool>> Criteria
        ////{
        ////    get => _criteria; 
        ////}

        public Expression<Func<T, object>>? OrderBy
        {
            get;
            private set;
        }

        public Expression<Func<T, object>>? OrderByDescending
        {
            get;
            private set;
        }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public List<string> IncludesStrings { get; } = new List<string>();

        public bool IsDistinct
        {
            get;
            private set;
        }

        public int Take { get; set; }

        public int Skip { get; set; }

        public bool IsPagingEndabled { get; set; }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected void AddIncludes(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddIncludes(string includeString)
        {
            IncludesStrings.Add(includeString);
        }

        protected void ApplyDistinct()
        {
            IsDistinct = true;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEndabled = true;
        }

        public IQueryable<T> ApplyCriteria(IQueryable<T> query)
        {
            if (Criteria is not null)
            {
                query = query.Where(Criteria);
            }

            return query;
        }
    }

    public class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
    {
        protected BaseSpecification() : this(null) { }

        // Constructor normal
        public BaseSpecification(Expression<Func<T, bool>>? criteria)
            : base(criteria)
        {
        }

        public Expression<Func<T, TResult>>? Select { get; private set; }

        protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
        {
            Select = selectExpression;
        }
    }

    ////public class BaseSpecification<T, TResult>(Expression<Func<T, bool>> criteria) : BaseSpecification<T>(criteria), ISpecification<T, TResult>
    ////{
    ////    public Expression<Func<T, TResult>>? Select { get; private set; }

    ////    protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
    ////    {
    ////        Select = selectExpression;
    ////    }
    ////}
}
