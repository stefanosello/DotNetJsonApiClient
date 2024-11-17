using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JsonApiClient.Attributes;
using JsonApiClient.Extensions;
using Newtonsoft.Json;

namespace JsonApiClient.Statements.ExpressionVisitors;

internal class AttributeSelectorExpressionVisitor : ExpressionVisitor
{
    private readonly StringBuilder _sb = new();

    public static string? VisitExpression(Expression? expression)
    {
        var visitor = new AttributeSelectorExpressionVisitor();
        var node = visitor.Visit(expression);
        return node is null ? null : visitor._sb.ToString();
    }

    public override Expression? Visit(Expression? node)
    {
        return node switch
        {
            null => null,
            MemberExpression member => VisitMember(member),
            _ => throw new InvalidExpressionException(
                "Subresource selectors can only be expressions of type MemberExpression or MethodCallExpression.")
        };
    }
    
    protected override Expression VisitMember(MemberExpression node)
    {
        var parent = node.Expression as MemberExpression;
        var property = (JAttrAttribute?)node.Member.GetCustomAttribute(typeof(JAttrAttribute));
        if (property is null)
            throw new InvalidExpressionException(
                $"Member {node.Member.Name} is not decorated with attribute ${nameof(JAttrAttribute)}, hence it cannot be interpreted as a json:api attribute.");
        var jsonProperty = (JsonPropertyAttribute?)node.Member.GetCustomAttribute(typeof(JsonPropertyAttribute));
        var propertyName = jsonProperty?.PropertyName ?? node.Member.Name.Uncapitalize();
        if (parent is null)
            _sb.Append(propertyName);
        else
        {
            VisitMember(parent);
            _sb.Append($".{propertyName}");
        }

        return node;
    }
}