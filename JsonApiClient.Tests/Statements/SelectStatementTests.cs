using FluentAssertions;
using JsonApiClient.Statements;
using JsonApiClient.Tests.Models;

namespace JsonApiClient.Tests.Statements;

public class SelectStatementTests
{
    [Fact]
    public void Parse_ShouldReturnValidFieldsString_WhenValidExpression()
    {
        SelectStatement<Author> statement = new SelectStatement<Author>(a => new
        {
            a.FirstName,
            a.LastName
        });

        var result = statement.Translate();

        result.Key.Should().Be("fields[author]");
        result.Value.Should().Be("firstName,lastName");
    }
}