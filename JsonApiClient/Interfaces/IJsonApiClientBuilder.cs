using System.Linq.Expressions;
using JsonApiDotNetCore.Resources;

namespace JsonApiClient.Interfaces;

public interface IJsonApiClientBuilder<TRootEntity> where TRootEntity : class, IIdentifiable
{
    IJsonApiClientBuilder<TRootEntity> SetHttpClient(IHttpClientFactory httpClientFactory, string httpClientId);

    IJsonApiClientBuilder<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement)
        where TEntity : class, IIdentifiable;

    IJsonApiClient<TRootEntity> Build();
}