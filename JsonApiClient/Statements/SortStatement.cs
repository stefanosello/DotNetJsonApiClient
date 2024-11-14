using System.Linq.Expressions;
using JsonApiClient.Enums;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements.ExpressionVisitors;

namespace JsonApiClient.Statements;

internal class SortStatement<TEntity,TRoot>(Expression<Func<TRoot,object>>? resourceSelector, Expression<Func<TEntity,object>> expression, SortDirection direction) : IStatement
    where TEntity : class, IJsonApiResource
    where TRoot : class, IJsonApiResource
{
    public KeyValuePair<string,string> Translate()
    {
        var targetResourceName = SubresourceSelectorExpressionVisitor.VisitExpression(resourceSelector?.Body);
        var propertyName = AttributeSelectorExpressionVisitor.VisitExpression(expression.Body);
        var directionPrefix = direction == SortDirection.Ascending ? "" : "-";
        var key = targetResourceName is null ? "sort" : $"sort[{targetResourceName}]";
        return new KeyValuePair<string, string>(key,$"{directionPrefix}{propertyName}");
    }
}