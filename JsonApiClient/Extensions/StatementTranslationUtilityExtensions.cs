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
        var attribute = (JAttrAttribute?)expression.Member.GetCustomAttribute(typeof(JAttrAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Member {expression.Member.Name} is not decorated with attribute ${nameof(JAttrAttribute)}, hence it cannot be interpreted as a json:api attribute.");
        return attribute.AttributeName ?? expression.Member.Name.Uncapitalize();
    }
    
    public static string GetRelationshipName(this MemberExpression expression)
    {
        var attribute = (JRelAttribute?)expression.Member.GetCustomAttribute(typeof(JRelAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Member {expression.Member.Name} is not decorated with attribute ${nameof(JRelAttribute)}, hence it cannot be interpreted as a json:api relationship.");
        return attribute.RelationshipName ?? expression.Member.Name.Uncapitalize();
    }
    
    public static string GetResourceName(this Type type)
    {
        var attribute = (JResAttribute?)type.GetCustomAttribute(typeof(JResAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Type {nameof(type)} is not decorated with attribute ${nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
        return attribute.ResourceName ?? type.Name.Uncapitalize();
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