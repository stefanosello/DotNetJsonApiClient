using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using JsonApiClient.Attributes;
using Newtonsoft.Json;

namespace JsonApiClient.Extensions;

internal static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, string> _resourceNameCache = new();
    private static readonly ConcurrentDictionary<Type, string> _resourceHttpClientIdCache = new();
    private static readonly ConcurrentDictionary<Type, string> _resourceNamespaceCache = new();
    
    internal static string GetResourceName(this Type type)
    {
        return _resourceNameCache.GetOrAdd(type, t =>
        {
            var attribute = (JResAttribute?)t.GetCustomAttribute(typeof(JResAttribute));
            if (attribute is null)
                throw new InvalidExpressionException(
                    $"Type {nameof(t)} is not decorated with attribute {nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
            var jsonProperty = (JsonPropertyAttribute?)t.GetCustomAttribute(typeof(JsonPropertyAttribute));
            return jsonProperty?.PropertyName ?? t.Name.Uncapitalize();
        });
    }
    
    internal static string GetResourceHttpClientId(this Type type)
    {
        return _resourceHttpClientIdCache.GetOrAdd(type, t =>
        {
            var attribute = (JResAttribute?)t.GetCustomAttribute(typeof(JResAttribute));
            if (attribute is null)
                throw new InvalidExpressionException(
                    $"Type {nameof(t)} is not decorated with attribute {nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
            return attribute.ClientId;
        });
    }
    
    internal static string GetResourceNamespace(this Type type)
    {
        return _resourceNamespaceCache.GetOrAdd(type, t =>
        {
            var attribute = (JResAttribute?)t.GetCustomAttribute(typeof(JResAttribute));
            if (attribute is null)
                throw new InvalidExpressionException(
                    $"Type {nameof(t)} is not decorated with attribute {nameof(JResAttribute)}, hence it cannot be interpreted as a json:api resource.");
            return attribute.ApiNamespace;
        });
    }
}