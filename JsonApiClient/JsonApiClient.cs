using System.Runtime.CompilerServices;
using JsonApiClient.Interfaces;
using JsonApiSerializer;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("JsonApiClient.Tests")]

namespace JsonApiClient;


public class JsonApiClient<TEntity>(IHttpClientFactory httpClientFactory) : IJsonApiClient<TEntity> where TEntity : class, IJsonApiResource
{
    public IJsonApiQueryClient<TEntity> Query => new JsonApiQueryClient<TEntity>(httpClientFactory);
}