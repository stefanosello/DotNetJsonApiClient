using System.Reflection;
using JsonApiClient.Attributes;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Models;

/// <summary>
/// The base class for all json:api resources.
/// It provides with base attributes and methods to properly handle the serialization and deserialization of
/// the resources.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier attribute (Id).</typeparam>
public class JResource<TIdentifier> : IJsonApiResource
{
    /// <summary>
    /// The identifier of the resource.
    /// </summary>
    public TIdentifier? Id { get; set; }
    /// <inheritdoc/>
    public string? Lid { get; set; }
    
    /// <inheritdoc/>
    public string Type => GetResourceType();
    /// <inheritdoc/>
    string IJsonApiResource.Id => Id?.ToString() ?? "";

    private string GetResourceType()
    {
        var jres = GetType().GetCustomAttribute<JResAttribute>(false) ?? throw new MissingAttributeException(
            $"Model {GetType()} is not decorated with the {nameof(JResAttribute)}, hence it can not be used as a json:api resource.");
        return jres.ResourceName ?? GetType().Name.Uncapitalize();
    }
}