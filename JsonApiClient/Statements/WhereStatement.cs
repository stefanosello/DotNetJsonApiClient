using System.Linq.Expressions;
using JsonApiClient.Exceptions;
using JsonApiClient.ExpressionVisitors;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class WhereStatement<TEntity>(Expression<Func<TEntity,bool>> expression) : IStatement where TEntity : class
{
    public KeyValuePair<string, string> Translate(string? targetResourceName = null)
    {
        var visitor = new JsonApiFilterExpressionVisitor();
        try
        {
            visitor.Visit(expression.Body);
        }
        catch (Exception e)
        {
            throw new StatementTranslationException(
                $"Unable to translate expression {expression} to 'filter' query parameter.", e);
        }
        
        string filterPropName = targetResourceName is null ? "filter" : $"filter[{targetResourceName}]";
        return new KeyValuePair<string, string>(filterPropName, visitor.ToString());
    }
}