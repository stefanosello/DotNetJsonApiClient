using FluentAssertions;
using JsonApiClient.Enums;
using JsonApiClient.Statements;
using JsonApiClient.Tests.Models;

namespace JsonApiClient.Tests.Statements;

public class SortStatementTests
{
    [Fact]
    public void Parse_ShouldReturnAscendingSinglePropertySortClause_WhenAscendingRootEntityPropetyGiven()
    {
        SortStatement<Author,Author> statement = new(null, a => a.LastName!, SortDirection.Ascending);

        var result = statement.Translate();

        result.Key.Should().Be("sort");
        result.Value.Should().Be("lastName");
    }
    
    [Fact]
    public void Parse_ShouldReturnDescendingSinglePropertySortClause_WhenDescendingRootEntityPropetyGiven()
    {
        SortStatement<Author,Author> statement = new(null, a => a.LastName!, SortDirection.Descending);

        var result = statement.Translate();

        result.Key.Should().Be("sort");
        result.Value.Should().Be("-lastName");
    }
    
    [Fact]
    public void Parse_ShouldReturnAscendingNestedPropertySortClause_WhenAscendingNestedEntityPropetyGiven()
    {
        SortStatement<Book,Author> statement = new(a => a.Books, b => b.Title!, SortDirection.Ascending);

        var result = statement.Translate();

        result.Key.Should().Be("sort[books]");
        result.Value.Should().Be("title");
    }
}