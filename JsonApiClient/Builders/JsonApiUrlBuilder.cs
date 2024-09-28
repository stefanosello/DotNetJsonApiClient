using JsonApiClient.Interfaces;

namespace JsonApiClient.Builders;

internal class JsonApiUrlBuilder
{
    private readonly ICollection<IStatement> _selectStatements = [];
    private readonly ICollection<IStatement> _whereStatements = [];
    private readonly ICollection<IStatement> _includeStatements = [];
    private readonly ICollection<IStatement> _orderByStatements = [];
    private readonly ICollection<IStatement> _pageSizeStatements = [];
    private readonly ICollection<IStatement> _pageNumberStatements = [];

    private IEnumerable<IStatement> Statements =>
    [
        .._selectStatements,
        .._whereStatements,
        .._includeStatements,
        .._orderByStatements,
        .._pageNumberStatements,
        .._pageSizeStatements
    ];
    
    internal string Build(string path)
    {
        List<string> processedStatements = [];
        
        foreach (var statement in Statements)
            statement.Validate();
        
        processedStatements.Add(BuildWithoutKeyAggregation(_includeStatements));
        processedStatements.Add(BuildWithoutKeyAggregation(_whereStatements));
        processedStatements.Add(BuildWithKeyAggregation(_selectStatements));
        processedStatements.Add(BuildWithKeyAggregation(_orderByStatements));
        processedStatements.Add(BuildWithKeyAggregation(_pageSizeStatements));
        processedStatements.Add(BuildWithKeyAggregation(_pageNumberStatements));

        var queryString = string.Join('&', processedStatements.Where(ps => !string.IsNullOrWhiteSpace(ps)));

        return $"{path}?{queryString}";
    }

    internal void AddSelectStatement(IStatement statement)
    {
        _selectStatements.Add(statement);
    }
    
    internal void AddWhereStatement(IStatement statement)
    {
        _whereStatements.Add(statement);
    }
    
    internal void AddIncludeStatement(IStatement statement)
    {
        _includeStatements.Add(statement);
    }
    
    internal void AddOrderByStatement(IStatement statement)
    {
        _orderByStatements.Add(statement);
    }
    
    internal void AddPageSizeStatement(IStatement statement)
    {
        _pageSizeStatements.Add(statement);
    }
    
    internal void AddPageNumberStatement(IStatement statement)
    {
        _pageNumberStatements.Add(statement);
    }

    private static string BuildWithKeyAggregation(IEnumerable<IStatement> statements)
    {
        var processedStatements = statements.Select(s => s.Translate());
        var statementsGroupedByKeys = processedStatements.ToLookup(s => s.Key, s => s.Value);
        var aggregatedStatements =
            statementsGroupedByKeys.Select(group => $"{group.Key}={string.Join(',', group)}");
        return string.Join('&', aggregatedStatements);
    }
    
    private static string BuildWithoutKeyAggregation(IEnumerable<IStatement> statements)
    {
        var processedStatements = statements
            .Select(s => s.Translate())
            .Select(kv => $"{kv.Key}={kv.Value}");
        return string.Join('&', processedStatements);
    }
}