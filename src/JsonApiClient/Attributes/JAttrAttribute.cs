namespace JsonApiClient.Attributes;

/// <summary>
/// Used to decorate those properties of a model or DTO that represent a <c>json:api</c> resource attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class JAttrAttribute : Attribute;