using System.Linq.Expressions;
using JsonApiClient.Enums;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements.ExpressionVisitors;

namespace JsonApiClient.Statements;

internal class PageStatement<TEntity,TRoot>(int paramValue, PaginationParameter parameter, Expression<Func<TRoot,IEnumerable<TEntity>>>? resourceSelector = null) : IStatement
    where TEntity : class, IJsonApiResource
    where TRoot : class, IJsonApiResource
{
    public KeyValuePair<string, string> Translate()
    {
        var targetResourceName = SubresourceSelectorExpressionVisitor.VisitExpression(resourceSelector?.Body);
        var value = targetResourceName is null ? paramValue.ToString() : $"{targetResourceName}:{paramValue}";
        var key = parameter == PaginationParameter.PageNumber ? "page[number]" : "page[size]";
        return new KeyValuePair<string, string>(key, value);
    }
}