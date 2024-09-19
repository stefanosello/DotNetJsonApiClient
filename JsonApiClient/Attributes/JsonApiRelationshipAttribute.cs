namespace JsonApiClient.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class JsonApiRelationshipAttribute(string? relationshipName = null) : Attribute
{
    public string? RelationshipName { get; } = relationshipName;
}