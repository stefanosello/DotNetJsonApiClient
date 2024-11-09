using System.Linq.Expressions;

namespace JsonApiClient.Interfaces;

public interface IJsonApiQueryClient<TRootEntity> where TRootEntity : class
{
    IJsonApiQueryClient<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement)
        where TEntity : class, IJsonApiResource;

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