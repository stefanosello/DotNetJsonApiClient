using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JsonApiClient.Attributes;
using JsonApiClient.Extensions;

namespace JsonApiClient.Statements.ExpressionVisitors;

internal class SubresourceSelectorExpressionVisitor : ExpressionVisitor
{
    private readonly StringBuilder _sb = new();
    private static readonly IEnumerable<string> AllowedSelectorMethods = ["Select", "SelectMany"];

    public static string? VisitExpression(Expression? expression)
    {
        if (expression is null)
            return null;
        
        var visitor = new SubresourceSelectorExpressionVisitor();
        visitor.Visit(expression);
        return visitor._sb.ToString();
    }

    public override Expression? Visit(Expression? node)
    {
        return node switch
        {
            null => null,
            MemberExpression member => VisitMember(member),
            MethodCallExpression methodCall => VisitMethodCall(methodCall),
            _ => throw new InvalidExpressionException(
                "Subresource selectors can only be expressions of type MemberExpression or MethodCallExpression.")
        };
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (AllowedSelectorMethods.Contains(node.Method.Name) && node.Arguments.Count == 2)
        {
            var member2Expression = node.Arguments[1] as LambdaExpression ?? throw new InvalidExpressionException(
                $"Expression of type {typeof(LambdaExpression)} expected, but #{node.Arguments[1].GetType().Name} found: {node.Arguments[1]}.");
            
            if (node.Arguments[0] is not MemberExpression member1)
                throw new InvalidExpressionException(
                    $"Expression of type {typeof(MemberExpression)} expected, but #{node.Arguments[0].GetType().Name} found: {node.Arguments[0]}.");
            
            VisitMember(member1);
            _sb.Append(".");
            
            var _ = member2Expression.Body switch
            {
                MemberExpression member => VisitMember(member),
                MethodCallExpression methodCall => VisitMethodCall(methodCall),
                _ => throw new InvalidExpressionException(
                    $"Expression of type {typeof(MemberExpression)} or {typeof(MethodCallExpression)} expected, but #{member2Expression.Body.GetType().Name} found: {member2Expression.Body}.")
            };

            return node;
        }

        throw new InvalidExpressionException(
            $"Expression of type {typeof(MethodCallExpression)} with method name between {string.Join(", ", AllowedSelectorMethods)} expected, but {node.Method.Name} found: {node}."); 
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        var parent = node.Expression as MemberExpression;
        var attribute = (JRelAttribute?)node.Member.GetCustomAttribute(typeof(JRelAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Member {node.Member.Name} is not decorated with attribute ${nameof(JRelAttribute)}, hence it cannot be interpreted as a json:api relationship.");
        var relName = attribute.RelationshipName ?? node.Member.Name.Uncapitalize();
        if (parent is null)
            _sb.Append(relName);
        else
        {
            VisitMember(parent);
            _sb.Append($".{relName}");
        }

        return node;
    }


}