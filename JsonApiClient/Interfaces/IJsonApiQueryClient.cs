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
    
    /// <summary>
    /// Builds the <c>filter[res]</c> part of the query string. It can be used to specify a filtering condition on the
    /// fields of the <typeparamref name="TEntity"/> resource type that will affect the set of <typeparamref name="TEntity"/>
    /// resources returned by the server. This means that each resource of type <typeparamref name="TEntity"/> in the
    /// server response will satisfy the filtering condition defined by the <paramref name="whereStatement"/> lambda function.
    /// </summary>
    /// <param name="whereStatement">A lambda function accepting a <typeparamref name="TEntity"/> as parameter
    /// and returning a boolean expression that will be used as the filtering condition.</param>
    /// <typeparam name="TEntity">The type of the resource for which to define the filtering condition.</typeparam>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and a set of related resources of class <c>Book</c>
    /// having resource type <c>books</c> and field <c>Title</c>. The following code snippet:
    /// The following code snippet:
    /// <code>
    /// queryClient.Where&lt;Book&gt;(b => b.Title == "The Hobbit");
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?filter[books]=Equals(title,'The Hobbit')
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> Where<TEntity>(Expression<Func<TEntity, bool>> whereStatement)
        where TEntity : class, IJsonApiResource;
    
    /// <summary>
    /// Builds the <c>filter</c> part of the query string. It can be used to specify a filtering condition on the
    /// fields of the <typeparamref name="TRootEntity"/> and resource type and its subresources, that will affect the set of
    /// <typeparamref name="TRootEntity"/> resources returned by the server.
    /// </summary>
    /// <param name="whereStatement">A lambda function accepting a <typeparamref name="TRootEntity"/> as parameter
    /// and returning a boolean expression that will be used as the filtering condition.</param>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and a field <c>Name</c>.
    /// The following code snippet:
    /// <code>
    /// queryClient.Where(a => a.Name == "J.R.R. Tolkien");
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?filter=Equals(name,'J.R.R. Tolkien')
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> Where(Expression<Func<TRootEntity, bool>> whereStatement);
    
    /// <summary>
    /// Builds the <c>include</c> part of the query string. It can be used to specify which subresources of
    /// <typeparamref name="TRootEntity"/> resource type should be returned by the server.
    /// </summary>
    /// <param name="includeStatement">A lambda function accepting a <typeparamref name="TRootEntity"/> as parameter
    /// and returning a <typeparamref name="TRootEntity"/> relationship property, that will eventually be included
    /// in the server response.</param>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and a relationship <c>Books</c> with an entity
    /// type <c>books</c>.
    /// The following code snippet:
    /// <code>
    /// queryClient.Include(a => a.Books);
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?include=books
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> Include(Expression<Func<TRootEntity, object>> includeStatement);
    
    /// <summary>
    /// Builds the <c>orderBy[res]</c> part of the query string. It can be used to specify how to sort in ascending order
    /// the instances of the specified <typeparamref name="TRootEntity"/> subresources set. The sorting is done based
    /// on the specified property of the <typeparamref name="TEntity"/> resource type.
    /// </summary>
    /// <param name="orderByStatement">A lambda function accepting a <typeparamref name="TEntity"/> as parameter
    /// and returning a <typeparamref name="TEntity"/> attribute property, that will be used to sort the records of
    /// the given subresource set.</param>
    /// <param name="subresourceSelector">A lambda function accepting a <typeparamref name="TRootEntity"/> as parameter
    /// and returning an enumerable of <typeparamref name="TEntity"/>, representing the set of subresource to sort.</param>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and a relationship <c>Books</c> with an entity
    /// type <c>books</c>. Each book also has the field <c>Title</c>.
    /// The following code snippet:
    /// <code>
    /// queryClient.OrderBy(b => b.Title, a => a.Books);
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?orderBy[books]=title
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> orderByStatement, Expression<Func<TRootEntity, IEnumerable<TEntity>>> subresourceSelector)
        where TEntity : class, IJsonApiResource;
    
    /// <summary>
    /// Builds the <c>orderBy[res]</c> part of the query string. It can be used to specify how to sort in ascending order
    /// the set of <typeparamref name="TRootEntity"/> instances. The sorting is done based
    /// on the specified property of the <typeparamref name="TRootEntity"/> resource type.
    /// </summary>
    /// <param name="orderByStatement">A lambda function accepting a <typeparamref name="TRootEntity"/> as parameter
    /// and returning a <typeparamref name="TRootEntity"/> attribute property, that will be used to sort the records of
    /// the given subresource set.</param>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and the <c>Name</c> field.
    /// The following code snippet:
    /// <code>
    /// queryClient.OrderBy(a => a.Name);
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?orderBy=name
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> OrderBy(Expression<Func<TRootEntity, object>> orderByStatement);
    
    /// <summary>
    /// Builds the <c>orderBy[res]</c> part of the query string. It can be used to specify how to sort in descending order
    /// the instances of the specified <typeparamref name="TRootEntity"/> subresources set. The sorting is done based
    /// on the specified property of the <typeparamref name="TEntity"/> resource type.
    /// </summary>
    /// <param name="orderByStatement">A lambda function accepting a <typeparamref name="TEntity"/> as parameter
    /// and returning a <typeparamref name="TEntity"/> attribute property, that will be used to sort the records of
    /// the given subresource set.</param>
    /// <param name="subresourceSelector">A lambda function accepting a <typeparamref name="TRootEntity"/> as parameter
    /// and returning an enumerable of <typeparamref name="TEntity"/>, representing the set of subresource to sort.</param>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and a relationship <c>Books</c> with an entity
    /// type <c>books</c>. Each book also has the field <c>Title</c>.
    /// The following code snippet:
    /// <code>
    /// queryClient.OrderByDescending(b => b.Title, a => a.Books);
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?orderBy[books]=-title
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> orderByStatement, Expression<Func<TRootEntity, IEnumerable<TEntity>>> subresourceSelector)
        where TEntity : class, IJsonApiResource;
    
    /// <summary>
    /// Builds the <c>orderBy[res]</c> part of the query string. It can be used to specify how to sort in descending order
    /// the set of <typeparamref name="TRootEntity"/> instances. The sorting is done based
    /// on the specified property of the <typeparamref name="TRootEntity"/> resource type.
    /// </summary>
    /// <param name="orderByStatement">A lambda function accepting a <typeparamref name="TRootEntity"/> as parameter
    /// and returning a <typeparamref name="TRootEntity"/> attribute property, that will be used to sort the records of
    /// the given subresource set.</param>
    /// <returns>This <see cref="IJsonApiQueryClient{TRootEntity}"/> instance.</returns>
    /// <example>
    /// Consider an <c>Author</c> class with resource type <c>authors</c> and the <c>Name</c> field.
    /// The following code snippet:
    /// <code>
    /// queryClient.OrderByDescending(a => a.Name);
    /// </code>
    /// will render the following query string:
    /// <code>
    /// ?orderBy=-name
    /// </code>
    /// considering that <c>Author</c> is the main resource type.
    /// </example>
    IJsonApiQueryClient<TRootEntity> OrderByDescending(Expression<Func<TRootEntity, object>> orderByStatement);
    
    IJsonApiQueryClient<TRootEntity> PageSize<TEntity>(int limit, Expression<Func<TRootEntity,IEnumerable<TEntity>>> resourceSelector) where TEntity : class, IJsonApiResource;

    IJsonApiQueryClient<TRootEntity> PageSize(int limit);

    IJsonApiQueryClient<TRootEntity> PageNumber<TEntity>(int number, Expression<Func<TRootEntity,IEnumerable<TEntity>>> resourceSelector) where TEntity : class, IJsonApiResource;

    IJsonApiQueryClient<TRootEntity> PageNumber(int number);

    Task<TRootEntity?> FindAsync(object id, CancellationToken cancellationToken = default);
    
    Task<TRootEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    Task<List<TRootEntity>> ToListAsync(CancellationToken cancellationToken = default);
}