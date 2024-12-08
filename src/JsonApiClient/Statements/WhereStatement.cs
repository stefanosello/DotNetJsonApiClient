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
        var queryString = FilterConditionExpressionVisitor.VisitExpression(expression.Body);
        var targetResourceName = typeof(TEntity) == typeof(TRoot) ? null : typeof(TEntity).GetResourceName();
        var filterPropName = targetResourceName is null ? "filter" : $"filter[{targetResourceName}]";
        return new KeyValuePair<string, string>(filterPropName, queryString);
    }
}