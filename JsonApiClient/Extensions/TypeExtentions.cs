using System.Data;
using System.Reflection;
using JsonApiClient.Attributes;
using Newtonsoft.Json;

namespace JsonApiClient.Extensions;

public static class TypeExtentions
{
    public static string GetResourceName(this Type type)
    {
        var attribute = (JResAttribute?)type.GetCustomAttribute(typeof(JResAttribute));
        if (attribute is null)
            throw new InvalidExpressionException(
                $"Type {nameof(type)} is not decorated with attribute ${nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
        var jsonProperty = (JsonPropertyAttribute?)type.GetCustomAttribute(typeof(JsonPropertyAttribute));
        return jsonProperty?.PropertyName ?? type.Name.Uncapitalize();
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