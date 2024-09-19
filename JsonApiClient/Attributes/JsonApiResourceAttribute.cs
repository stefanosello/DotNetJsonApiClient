namespace JsonApiClient.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class JsonApiResourceAttribute(string apiNamespace, string? resourceNameName = null) : Attribute
{
    public string ApiNamespace { get; } = apiNamespace;

    public string? ResourceName { get; } = resourceNameName;
    
}