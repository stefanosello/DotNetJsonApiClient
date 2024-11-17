using System.Runtime.CompilerServices;
using JsonApiClient.Clients;
using JsonApiClient.Interfaces;

[assembly: InternalsVisibleTo("JsonApiClient.Tests")]

namespace JsonApiClient;

/// <summary>
/// The main entry point to the library. It provides with a method to get the query client used to make read requests to a
/// <c>json:api</c> compliant API for the specified main resource type.
/// </summary>
/// <param name="httpClientFactory"></param>
public class JsonApiClient(IHttpClientFactory httpClientFactory) : IJsonApiClient
{
    /// <inheritdoc/>
    public IJsonApiQueryClient<TEntity> Query<TEntity>() where TEntity : class, IJsonApiResource =>
        new JsonApiQueryClient<TEntity>(httpClientFactory);
}