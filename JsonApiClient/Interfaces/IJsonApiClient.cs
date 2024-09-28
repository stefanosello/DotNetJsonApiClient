namespace JsonApiClient.Interfaces;

public interface IJsonApiClient<TEntity> where TEntity : class
{
    IJsonApiQueryClient<TEntity> Query { get; }
}