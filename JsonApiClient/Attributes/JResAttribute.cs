namespace JsonApiClient.Attributes;

/// <summary>
/// Used to decorate models or DTOs representing a json:api resource.
/// </summary>
/// <param name="clientId">The name of the named http client as configured in the application program.cs file</param>
/// <param name="apiNamespace">The namespace (i.e. the trailing part of the path) of the resource api url</param>
/// <param name="resourceName">The name of the resources. If not give, the name of the class is taken into account instead.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class JResAttribute(string clientId, string apiNamespace, string? resourceName = null) : Attribute
{
    /// <summary>
    /// The name of the named http client, as configured in the application program.cs file
    /// </summary>
    public string ClientId { get; } = clientId;
    /// <summary>
    /// The resource's api namespace
    /// </summary>
    public string ApiNamespace { get; } = apiNamespace;
    /// <summary>
    /// The resource name, if different from the class name
    /// </summary>
    public string? ResourceName { get; } = resourceName;
    
}