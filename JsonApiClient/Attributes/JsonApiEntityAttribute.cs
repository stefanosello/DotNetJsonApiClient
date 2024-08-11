namespace JsonApiClient.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class JsonApiEntityAttribute : Attribute
{
    public string JsonApiNamespace { get; }

    public string? JsonApiResource { get; }

    public JsonApiEntityAttribute(string jsonApiNamespace, string? jsonApiResource = null)
    {
        JsonApiNamespace = jsonApiNamespace;
        JsonApiResource = jsonApiResource;
    }
}