namespace JsonApiClient.Interfaces;

public interface IJsonApiClient<TEntity> where TEntity : class
{
    Task<TEntity?> GetAsync(CancellationToken cancellationToken = default);
}