namespace JsonApiClient.Attributes;

/// <summary>
/// Used to decorate those properties of a model or DTO that represent a relationship to another <c>json:api</c> resource.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class JRelAttribute : Attribute;