using JsonApiClient.Enums;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class PageStatement<TEntity,TRoot>(int paramValue, PaginationParameter parameter) : IStatement
    where TEntity : class, IJsonApiResource
    where TRoot : class, IJsonApiResource
{
    public KeyValuePair<string, string> Translate()
    {
        var targetResourceName = typeof(TEntity) == typeof(TRoot) ? null : typeof(TEntity).GetResourceName();
        var value = targetResourceName is null ? paramValue.ToString() : $"{targetResourceName}:{paramValue}";
        var key = parameter == PaginationParameter.PageNumber ? "page[number]" : "page[size]";
        return new KeyValuePair<string, string>(key, value);
    }

    public void Validate()
    {
        // NOTHING TO VALIDATE
    }
}