using FluentAssertions;
using JsonApiClient.Exceptions;
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
    
    [Fact]
    public void Parse_ShouldThrowParsinError_WhenWrongExpressionType()
    {
        SelectStatement<Author> statement = new SelectStatement<Author>(a => a.FirstName);

        var act = () => statement.Validate();

        act.Should().Throw<StatementTranslationException>().And.Message.Should().Be("Invalid expression body type. Expected: NewExpression. Found: MemberAccess");
    }
    
    [Fact]
    public void Parse_ShouldThrowParsinError_WhenWrongArgumentType()
    {
        SelectStatement<Author> statement = new SelectStatement<Author>(a => new
        {
            a.FirstName,
            b = "ciao"
        });

        var act = () => statement.Validate();

        act.Should().Throw<StatementTranslationException>().And.Message.Should().Be("Invalid expression body argument type. Expected: MemberExpression. Found: Constant");
    }
    
    [Fact]
    public void Parse_ShouldThrowParsinError_WhenInvalidMemberName()
    {
        var c = new { prop1 = 4 };
        
        SelectStatement<Author> statement = new SelectStatement<Author>(a => new
        {
            a.FirstName,
            c.prop1
        });

        var act = () => statement.Validate();

        act.Should().Throw<StatementTranslationException>().And.Message.Should().Be("Invalid member name: member prop1 is not a valid property name for Author");
    }
}