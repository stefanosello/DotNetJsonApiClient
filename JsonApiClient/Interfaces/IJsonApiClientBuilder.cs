using System.Linq.Expressions;

namespace JsonApiClient.Interfaces;

public interface IJsonApiClientBuilder<TRootEntity> where TRootEntity : class
{
    IJsonApiClientBuilder<TRootEntity> SetHttpClient(IHttpClientFactory httpClientFactory, string httpClientId);

    IJsonApiClientBuilder<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement)
        where TEntity : class;

    IJsonApiClient<TRootEntity> Build();
}