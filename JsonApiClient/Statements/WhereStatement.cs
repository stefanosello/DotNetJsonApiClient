using System.Linq.Expressions;
using JsonApiClient.Exceptions;
using JsonApiClient.ExpressionVisitors;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class WhereStatement<TEntity,TRoot>(Expression<Func<TEntity,bool>> expression) : IStatement
    where TEntity : class, IJsonApiResource
    where TRoot : class, IJsonApiResource
{
    public KeyValuePair<string, string> Translate()
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
        
        var targetResourceName = typeof(TEntity) == typeof(TRoot) ? null : typeof(TEntity).GetResourceName();
        var filterPropName = targetResourceName is null ? "filter" : $"filter[{targetResourceName}]";
        return new KeyValuePair<string, string>(filterPropName, visitor.ToString());
    }
    
    public void Validate() { }
}