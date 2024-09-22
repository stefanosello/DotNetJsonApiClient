using JsonApiClient.Enums;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class PageStatement<TEntity>(int paramValue, PaginationParameter parameter) : IStatement where TEntity : class
{
    public KeyValuePair<string, string> Translate(string? targetResourceName = null)
    {
        var value = targetResourceName is null ? paramValue.ToString() : $"{targetResourceName}:{paramValue}";
        var key = parameter == PaginationParameter.PageNumber ? "page[number]" : "page[size]";
        return new KeyValuePair<string, string>(key, value);
    }

    public void Validate()
    {
        // NOTHIN TO VALIDATE
    }
}