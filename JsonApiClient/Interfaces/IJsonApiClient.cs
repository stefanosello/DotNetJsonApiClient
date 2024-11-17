namespace JsonApiClient.Interfaces;

/// <summary>
/// Provides a method to get the query client used to make read requests to a <c>json:api</c> compliant API.
/// </summary>
public interface IJsonApiClient
{
   /// <summary>
   /// Provides the query client for the specified resource entity.
   /// </summary>
   /// <typeparam name="TEntity">The main resource to be queried.</typeparam>
   /// <returns>The <see cref="IJsonApiQueryClient&lt;TEntity&gt;"/> query client for the specified resource.</returns>
   IJsonApiQueryClient<TEntity> Query<TEntity>() where TEntity : class, IJsonApiResource;
}