using System.Runtime.CompilerServices;
using JsonApiClient.Interfaces;
using JsonApiDotNetCore.Resources;
using JsonApiSerializer;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("JsonApiClient.Tests")]

namespace JsonApiClient;


public class JsonApiClient<TEntity>(IHttpClientFactory httpClientFactory, string httpClientId, string url) : IJsonApiClient<TEntity> where TEntity : class, IIdentifiable
{
    private IHttpClientFactory _httpClientFactory = httpClientFactory;
    private string _httpClientId = httpClientId;
    
    internal string Url { get; } = url;

    public async Task<TEntity?> GetAsync(CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient(httpClientId);
        HttpResponseMessage httpResponse = await httpClient.GetAsync(Url, cancellationToken);
        string reponseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        List<TEntity>? result =
            JsonConvert.DeserializeObject<List<TEntity>>(reponseContent, new JsonApiSerializerSettings());
        return result?.FirstOrDefault();
    }
}