using System.Linq.Expressions;

namespace JsonApiClient.Interfaces;

public interface IJsonApiClientBuilder<TRootEntity> where TRootEntity : class
{
    IJsonApiClientBuilder<TRootEntity> SetHttpClient(IHttpClientFactory httpClientFactory, string httpClientId);

    IJsonApiClientBuilder<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement)
        where TEntity : class;

    IJsonApiClientBuilder<TRootEntity> Select(Expression<Func<TRootEntity, object>> selectStatement);

    IJsonApiClientBuilder<TRootEntity> Where<TEntity>(Expression<Func<TEntity, bool>> whereStatement)
        where TEntity : class;
    
    IJsonApiClientBuilder<TRootEntity> Where(Expression<Func<TRootEntity, bool>> whereStatement);

    IJsonApiClientBuilder<TRootEntity> Include<TEntity>(Expression<Func<TEntity, object>> includeStatement)
        where TEntity : class;
    
    IJsonApiClientBuilder<TRootEntity> Include(Expression<Func<TRootEntity, object>> includeStatement);

    IJsonApiClientBuilder<TRootEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> orderByStatement)
        where TEntity : class;
    
    IJsonApiClientBuilder<TRootEntity> OrderBy(Expression<Func<TRootEntity, object>> orderByStatement);

    IJsonApiClientBuilder<TRootEntity> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> orderByStatement)
        where TEntity : class;
    
    IJsonApiClientBuilder<TRootEntity> OrderByDescending(Expression<Func<TRootEntity, object>> orderByStatement);

    IJsonApiClientBuilder<TRootEntity> PageSize<TEntity>(int limit) where TEntity : class;

    IJsonApiClientBuilder<TRootEntity> PageSize(int limit);

    IJsonApiClientBuilder<TRootEntity> PageNumber<TEntity>(int number) where TEntity : class;

    IJsonApiClientBuilder<TRootEntity> PageNumber(int number);

    IJsonApiClient<TRootEntity> Build();
}