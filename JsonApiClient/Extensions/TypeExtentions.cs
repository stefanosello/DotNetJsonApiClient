using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using JsonApiClient.Attributes;

namespace JsonApiClient.Extensions;

public static class TypeExtentions
{
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