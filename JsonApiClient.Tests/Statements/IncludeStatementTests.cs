using FluentAssertions;
using JsonApiClient.Statements;
using JsonApiClient.Tests.Models;

namespace JsonApiClient.Tests.Statements;

public class IncludeStatementTests
{
    [Fact]
    public void Parse_ShouldReturnValidIncludeField_WhenValidExpression()
    {
        var statement = new IncludeStatement<Author>(a => a.Books);

        var result = statement.Translate();

        result.Key.Should().Be("include");
        result.Value.Should().Be("books");
    }
    
    [Fact]
    public void Parse_ShouldReturnValidIncludeField_WhenIncludeChain()
    {
        var statement = new IncludeStatement<Author>(a => a.Books.Select(b => b.TagsBooks));

        var result = statement.Translate();

        result.Key.Should().Be("include");
        result.Value.Should().Be("books.tagsBooks");
    }
    
    [Fact]
    public void Parse_ShouldReturnValidIncludeField_WhenIncludeChainWithMoreThenANestedCollection()
    {
        var statement = new IncludeStatement<Author>(a => a.Books.Select(b => b.TagsBooks.Select(tb => tb.Tag)));

        var result = statement.Translate();

        result.Key.Should().Be("include");
        result.Value.Should().Be("books.tagsBooks.tag");
    }
    
    [Fact]
    public void Parse_ShouldReturnValidIncludeField_WhenIncludeChainWithACircularRelationship()
    {
        var statement = new IncludeStatement<Author>(a => a.Books.Select(b => b.TagsBooks.Select(tb => tb.Tag.TagsBooks.Select(tb2 => tb2.Book))));

        var result = statement.Translate();

        result.Key.Should().Be("include");
        result.Value.Should().Be("books.tagsBooks.tag.tagsBooks.book");
    }
}