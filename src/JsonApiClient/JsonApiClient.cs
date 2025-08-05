using System.Runtime.CompilerServices;
using JsonApiClient.Clients;
using JsonApiClient.Interfaces;

[assembly: InternalsVisibleTo("JsonApiClient.Tests")]

namespace JsonApiClient;

/// <summary>
/// The main entry point to the library. It provides with a method to get the query client used to make read requests to a
/// <c>json:api</c> compliant API for the specified main resource type.
/// </summary>
/// <param name="httpClientFactory">The HTTP client factory for creating HTTP clients.</param>
/// <param name="options">Configuration options for the client.</param>
public class JsonApiClient(IHttpClientFactory httpClientFactory, JsonApiClientOptions? options = null) : IJsonApiClient
{
    private readonly JsonApiClientOptions _options = options ?? new JsonApiClientOptions();
    
    /// <inheritdoc/>
    public IJsonApiQueryClient<TEntity> Query<TEntity>() where TEntity : class, IJsonApiResource =>
        new JsonApiQueryClient<TEntity>(httpClientFactory, _options);
}