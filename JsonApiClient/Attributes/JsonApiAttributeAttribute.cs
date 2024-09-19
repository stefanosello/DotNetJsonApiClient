namespace JsonApiClient.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class JsonApiAttributeAttribute(string? attributeName = null) : Attribute
{
    public string? AttributeName { get; } = attributeName;
}