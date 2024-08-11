using JsonApiDotNetCore.Resources;

namespace JsonApiClient.Interfaces;

public interface IJsonApiClient<TEntity> where TEntity : class, IIdentifiable
{
    Task<TEntity?> GetAsync(CancellationToken cancellationToken = default);
}