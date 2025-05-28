using System.Linq.Expressions;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a specification for querying and filtering data of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>A specification encapsulates query criteria, ordering, inclusion of related entities, and
    /// pagination settings. It is designed to be applied to an <see cref="IQueryable{T}"/> to construct a query that
    /// matches the defined rules.</remarks>
    /// <typeparam name="T">The type of entity to which the specification applies.</typeparam>
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>>? Criteria { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludesStrings { get; }    // For ThenInclude
        bool IsDistinct { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEndabled { get; }
        IQueryable<T> ApplyCriteria(IQueryable<T> query);
    }

    /// <summary>
    /// Defines a specification for querying and projecting data of type <typeparamref name="T"/> into a result of type
    /// <typeparamref name="TResult"/>.
    /// </summary>
    /// <remarks>This interface extends <see cref="ISpecification{T}"/> by adding support for specifying a
    /// projection through the <see cref="Select"/> property. The projection determines how the queried data is
    /// transformed into the result type.</remarks>
    /// <typeparam name="T">The type of the entity being queried.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by the query.</typeparam>
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        Expression<Func<T, TResult>>? Select { get; }
    }
}
