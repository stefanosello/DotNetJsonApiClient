using System.Data;
using System.Linq.Expressions;
using JsonApiClient.Enums;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class PageStatement<TEntity,TRoot>(int paramValue, PaginationParameter parameter, Expression<Func<TRoot,IEnumerable<TEntity>>>? resourceSelector = null) : IStatement
    where TEntity : class, IJsonApiResource
    where TRoot : class, IJsonApiResource
{
    public KeyValuePair<string, string> Translate()
    {
        var targetResourceName = typeof(TEntity) == typeof(TRoot) ? null : EvaluateResourceSelector();
        var value = targetResourceName is null ? paramValue.ToString() : $"{targetResourceName}:{paramValue}";
        var key = parameter == PaginationParameter.PageNumber ? "page[number]" : "page[size]";
        return new KeyValuePair<string, string>(key, value);
    }

    public void Validate()
    {
        if (typeof(TEntity) != typeof(TRoot) && resourceSelector is null)
            throw new StatementTranslationException(
                "Can not paginate a related resource without a resourceSelector expression.");
    }

    private string EvaluateResourceSelector()
    {
        return resourceSelector!.Body switch
        {
            MemberExpression member => member.GetRelationshipName(),
            MethodCallExpression methodCall => methodCall.GetRelationshipsChain(),
            _ => throw new InvalidExpressionException(
                $"Expression of type {typeof(MemberExpression)} or {typeof(MethodCallExpression)} expected, but #{resourceSelector.GetType().Name} found: {resourceSelector}.")
        };
    }
}