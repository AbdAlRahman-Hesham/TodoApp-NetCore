using System.Linq.Expressions;

namespace TodoApp.Application.Specifications;

class ParameterRebinder : ExpressionVisitor
{
    readonly Dictionary<ParameterExpression, ParameterExpression> map;

    ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }

    protected override Expression VisitParameter(ParameterExpression p)
    {
        if (map.TryGetValue(p, out var replacement))
        {
            p = replacement;
        }
        return base.VisitParameter(p);
    }
}