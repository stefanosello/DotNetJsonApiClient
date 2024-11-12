using System.Linq.Expressions;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements.ExpressionVisitors;

namespace JsonApiClient.Statements;

internal class WhereStatement<TEntity,TRoot>(Expression<Func<TEntity,bool>> expression) : IStatement
    where TEntity : class, IJsonApiResource
    where TRoot : class, IJsonApiResource
{
    public KeyValuePair<string, string> Translate()
    {
        string queryString;
        try
        {
            queryString = WhereExpressionVisitor.VisitExpression(expression.Body);
        }
        catch (Exception e)
        {
            throw new StatementTranslationException(
                $"Unable to translate expression {expression} to 'filter' query parameter.", e);
        }
        
        var targetResourceName = typeof(TEntity) == typeof(TRoot) ? null : typeof(TEntity).GetResourceName();
        var filterPropName = targetResourceName is null ? "filter" : $"filter[{targetResourceName}]";
        return new KeyValuePair<string, string>(filterPropName, queryString);
    }
}