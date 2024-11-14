using System.Linq.Expressions;

namespace JsonApiClient.Interfaces;

/// <summary>
/// Interface for a query client that can be used to build the json:api formatted query string to retrieve data from
/// a json:api-compliant server. It exposes the main methods that can be used to build a query, plus the methods
/// to send the request to the server through an HTTP client.
/// </summary>
/// <typeparam name="TRootEntity">The main resource type requested by the query that is going to be built.</typeparam>
public interface IJsonApiQueryClient<TRootEntity> where TRootEntity : class
{
    /// <summary>
    /// Builds the <c>select[res]</c> part of the query string. It is used to specify which fields 
    /// should be returned by the server for the resources of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="selectStatement">A lambda function returning a new() object with the fields to be included in the
    /// <c>select[res]</c> query parameter.</param>
    /// <typeparam name="TEntity">The type of the resource for which to define the set of fields to retrieve.</typeparam>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider a <c>Book</c> class with resource type <c>books</c> with the fields <c>Title</c> and <c>Price</c>.
    /// The following code snippet:
    /// <code>
    /// queryClient.Select&lt;Book&gt;(b => new() { b.Title, b.Price });
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?select[books]=title,price
    /// </code>
    /// </example>
    IJsonApiQueryClient<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement)
        where TEntity : class, IJsonApiResource;
    
    /// <summary>
    /// Builds the <c>select</c> part of the query string. It is used to specify which fields of the main resource
    /// of type <typeparamref name="TRootEntity"/> should be returned by the server.
    /// </summary>
    /// <param name="selectStatement">A lambda function returning a new() object with the fields to be included in the
    /// <c>select</c> query parameter.</param>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and fields <c>Name</c> and <c>DateOfBirth</c>.
    /// The following code snippet:
    /// <code>
    /// queryClient.Select(a => new() { a.Name, b.DateOfBirth });
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?select=name,dateOfBirth
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> Select(Expression<Func<TRootEntity, object>> selectStatement);

    IJsonApiQueryClient<TRootEntity> Where<TEntity>(Expression<Func<TEntity, bool>> whereStatement)
        where TEntity : class, IJsonApiResource;
    
    IJsonApiQueryClient<TRootEntity> Where(Expression<Func<TRootEntity, bool>> whereStatement);

    IJsonApiQueryClient<TRootEntity> Include<TEntity>(Expression<Func<TEntity, object>> includeStatement)
        where TEntity : class, IJsonApiResource;
    
    IJsonApiQueryClient<TRootEntity> Include(Expression<Func<TRootEntity, object>> includeStatement);

    IJsonApiQueryClient<TRootEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> orderByStatement, Expression<Func<TRootEntity, object>> subresourceSelector)
        where TEntity : class, IJsonApiResource;
    
    IJsonApiQueryClient<TRootEntity> OrderBy(Expression<Func<TRootEntity, object>> orderByStatement);

    IJsonApiQueryClient<TRootEntity> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> orderByStatement, Expression<Func<TRootEntity, object>> subresourceSelector)
        where TEntity : class, IJsonApiResource;
    
    IJsonApiQueryClient<TRootEntity> OrderByDescending(Expression<Func<TRootEntity, object>> orderByStatement);

    IJsonApiQueryClient<TRootEntity> PageSize<TEntity>(int limit, Expression<Func<TRootEntity,IEnumerable<TEntity>>> resourceSelector) where TEntity : class, IJsonApiResource;

    IJsonApiQueryClient<TRootEntity> PageSize(int limit);

    IJsonApiQueryClient<TRootEntity> PageNumber<TEntity>(int number, Expression<Func<TRootEntity,IEnumerable<TEntity>>> resourceSelector) where TEntity : class, IJsonApiResource;

    IJsonApiQueryClient<TRootEntity> PageNumber(int number);

    Task<TRootEntity?> FindAsync(object id, CancellationToken cancellationToken = default);
    
    Task<TRootEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    Task<List<TRootEntity>> ToListAsync(CancellationToken cancellationToken = default);
}