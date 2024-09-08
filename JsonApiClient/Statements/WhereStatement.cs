using System.Linq.Expressions;
using JsonApiClient.ExpressionVisitors;

namespace JsonApiClient.Statements;

public class WhereStatement<TEntity>(Expression<Func<TEntity,bool>> expression) : IStatement where TEntity : class
{
    public KeyValuePair<string, string> Translate(string? targetResourceName = null)
    {
        var visitor = new JsonApiFilterExpressionVisitor();
        visitor.Visit(expression.Body);
        string filterPropName = targetResourceName is null ? "filter" : $"filter[{targetResourceName}]";
        return new KeyValuePair<string, string>(filterPropName, visitor.ToString());
    }
}