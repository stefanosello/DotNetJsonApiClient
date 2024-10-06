using FluentAssertions;
using JsonApiClient.Enums;
using JsonApiClient.Statements;
using JsonApiClient.Tests.Models;

namespace JsonApiClient.Tests.Statements;

public class PageStatementTests
{
    [Fact]
    public void Parse_ShouldReturnValidPageStatement_WhenPrimaryResource()
    {
        var statement = new PageStatement<Author,Author>(1, PaginationParameter.PageNumber);

        var result = statement.Translate();

        result.Key.Should().Be("page[number]");
        result.Value.Should().Be("1");
    }
    
    [Fact]
    public void Parse_ShouldReturnValidPageStatement_WhenSecondaryResource()
    {
        var statement = new PageStatement<Book,Author>(1, PaginationParameter.PageNumber, a => a.Books);

        var result = statement.Translate();

        result.Key.Should().Be("page[number]");
        result.Value.Should().Be("books:1");
    }
    
    [Fact]
    public void Parse_ShouldReturnValidPageStatement_WhenSecondaryResourceWithAttributeChain()
    {
        var statement = new PageStatement<Tag,Author>(1, PaginationParameter.PageNumber, a => a.Books.SelectMany(b => b.TagsBooks.Select(tb => tb.Tag)));

        var result = statement.Translate();

        result.Key.Should().Be("page[number]");
        result.Value.Should().Be("books.tagsBooks.tag:1");
    }
}