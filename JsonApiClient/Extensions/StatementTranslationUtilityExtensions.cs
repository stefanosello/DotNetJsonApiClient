using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JsonApiClient.Attributes;

namespace JsonApiClient.Extensions;

public static class StatementTranslationUtilityExtensions
{
    public static string GetAttributeName(this MemberExpression expression)
    {
        var parent = expression.Expression is MemberExpression parentMember ? GetAttributeName(parentMember) : null;
        var attribute = (JAttrAttribute?)expression.Member.GetCustomAttribute(typeof(JAttrAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Member {expression.Member.Name} is not decorated with attribute ${nameof(JAttrAttribute)}, hence it cannot be interpreted as a json:api attribute.");
        var attributeName = attribute.AttributeName ?? expression.Member.Name.Uncapitalize();
        return parent is null ? attributeName : $"{parent}.{attributeName}";
    }
    
    public static string GetRelationshipName(this MemberExpression expression)
    {
        var parent = expression.Expression is MemberExpression parentMember ? GetRelationshipName(parentMember) : null;
        var attribute = (JRelAttribute?)expression.Member.GetCustomAttribute(typeof(JRelAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Member {expression.Member.Name} is not decorated with attribute ${nameof(JRelAttribute)}, hence it cannot be interpreted as a json:api relationship.");
        var attributeName = attribute.RelationshipName ?? expression.Member.Name.Uncapitalize();
        return parent is null ? attributeName : $"{parent}.{attributeName}";
    }
    
    public static string GetRelationshipsChain(this MethodCallExpression expression)
    {
        string[] allowedMethodNames = ["Select", "SelectMany"];
        if (allowedMethodNames.Contains(expression.Method.Name) && expression.Arguments.Count == 2)
        {
            var member2Expression = expression.Arguments[1] as LambdaExpression ?? throw new InvalidExpressionException(
                $"Expression of type {typeof(LambdaExpression)} expected, but #{expression.Arguments[1].GetType().Name} found: {expression.Arguments[1]}.");
            var member2RelationshipName = member2Expression.Body switch
            {
                MemberExpression member => member.GetRelationshipName(),
                MethodCallExpression methodCall => methodCall.GetRelationshipsChain(),
                _ => throw new InvalidExpressionException(
                    $"Expression of type {typeof(MemberExpression)} or {typeof(MethodCallExpression)} expected, but #{member2Expression.Body.GetType().Name} found: {member2Expression.Body}.")
            };

            if (expression.Arguments[0] is not MemberExpression member1)
                throw new InvalidExpressionException(
                    $"Expression of type {typeof(MemberExpression)} expected, but #{expression.Arguments[0].GetType().Name} found: {expression.Arguments[0]}.");

            return $"{member1.GetRelationshipName()}.{member2RelationshipName}";
        }

        throw new InvalidExpressionException(
            $"Expression of type {typeof(MethodCallExpression)} with method name between {string.Join(", ", allowedMethodNames)} expected, but {expression.Method.Name} found: {expression}."); 
    }
    
    public static string GetResourceName(this Type type)
    {
        var attribute = (JResAttribute?)type.GetCustomAttribute(typeof(JResAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Type {nameof(type)} is not decorated with attribute ${nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
        return attribute.ResourceName ?? type.Name.Uncapitalize();
    }
    
    public static string GetResourceHttpClientId(this Type type)
    {
        var attribute = (JResAttribute?)type.GetCustomAttribute(typeof(JResAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Type {nameof(type)} is not decorated with attribute ${nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
        return attribute.ClientId;
    }
    
    public static string GetResourceNamespace(this Type type)
    {
        var attribute = (JResAttribute?)type.GetCustomAttribute(typeof(JResAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Type {nameof(type)} is not decorated with attribute ${nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
        return attribute.ApiNamespace;
    }
}