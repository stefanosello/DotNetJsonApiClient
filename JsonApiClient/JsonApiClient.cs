using System.Runtime.CompilerServices;
using JsonApiClient.Interfaces;
using JsonApiSerializer;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("JsonApiClient.Tests")]

namespace JsonApiClient;


public class JsonApiClient<TEntity>(IHttpClientFactory httpClientFactory, string httpClientId, string url) : IJsonApiClient<TEntity> where TEntity : class
{
    public async Task<TEntity?> GetAsync(CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient(httpClientId);
        var httpResponse = await httpClient.GetAsync(url, cancellationToken);
        var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        List<TEntity>? result =
            JsonConvert.DeserializeObject<List<TEntity>>(responseContent, new JsonApiSerializerSettings());
        return result?.FirstOrDefault();
    }
}