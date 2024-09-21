namespace JsonApiClient.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class JAttrAttribute(string? attributeName = null) : Attribute
{
    public string? AttributeName { get; } = attributeName;
}