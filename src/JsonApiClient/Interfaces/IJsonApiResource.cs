namespace JsonApiClient.Interfaces;

/// <summary>
/// Mandatory properties for json:api resources.
/// </summary>
public interface IJsonApiResource
{
    /// <summary>
    /// The Id of the resource.
    /// Since a new resource (not already created) could not have an Id, the property is nullable.
    /// </summary>
    string? Id { get; }
    /// <summary>
    /// The type (or public name) of the resource.
    /// </summary>
    string Type { get; }
    /// <summary>
    /// The local id of the resource, considered only on new resources since already persisted resources do have an Id.
    /// </summary>
    string? Lid { get; }
}