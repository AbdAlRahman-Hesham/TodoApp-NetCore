using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoApp.Application.Specifications;

public class BaseSpecification<T> : ISpecification<T> where T : class 
{
    public Expression<Func<T, bool>>? Criteria { get; set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public BaseSpecification() { }

    public BaseSpecification(Expression<Func<T, bool>>? criteria)
    {
        Criteria = criteria;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
        => Includes.Add(includeExpression);

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        => OrderBy = orderByExpression;

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        => OrderByDescending = orderByDescExpression;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected void ApplySort(Expression<Func<T, object>> expression, bool descending)
    {
        if (descending)
            ApplyOrderByDescending(expression);
        else
            ApplyOrderBy(expression);
    }
    public virtual IQueryable<T> Apply(IQueryable<T> query)
    {
        if (Criteria != null)
            query = query.Where(Criteria);

        // Apply includes - modified to use EF Core's Include
        query = Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // Ordering
        if (OrderBy != null)
            query = query.OrderBy(OrderBy);
        else if (OrderByDescending != null)
            query = query.OrderByDescending(OrderByDescending);

        // Paging
        if (IsPagingEnabled)
            query = query.Skip(Skip).Take(Take);

        return query;
    }
}
