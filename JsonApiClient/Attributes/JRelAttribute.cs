namespace JsonApiClient.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class JRelAttribute(string? relationshipName = null) : Attribute
{
    public string? RelationshipName { get; } = relationshipName;
}