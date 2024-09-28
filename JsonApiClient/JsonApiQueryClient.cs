using System.Linq.Expressions;
using JsonApiClient.Builders;
using JsonApiClient.Enums;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements;
using JsonApiSerializer;
using Newtonsoft.Json;

namespace JsonApiClient;

internal class JsonApiQueryClient<TRootEntity>(IHttpClientFactory httpClientFactory) : IJsonApiQueryClient<TRootEntity> where TRootEntity : class, IJsonApiResource
{
    private readonly JsonApiUrlBuilder _urlBuilder = new();   
    public IJsonApiQueryClient<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddSelectStatement(new SelectStatement<TEntity>(selectStatement));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> Select(Expression<Func<TRootEntity, object>> selectStatement)
    {
        return Select<TRootEntity>(selectStatement);
    }
    
    public IJsonApiQueryClient<TRootEntity> Where<TEntity>(Expression<Func<TEntity, bool>> whereStatement) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddWhereStatement(new WhereStatement<TEntity,TRootEntity>(whereStatement));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> Where(Expression<Func<TRootEntity, bool>> whereStatement)
    {
        return Where<TRootEntity>(whereStatement);
    }
    
    public IJsonApiQueryClient<TRootEntity> Include<TEntity>(Expression<Func<TEntity, object>> includeStatement) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddIncludeStatement(new IncludeStatement<TEntity>(includeStatement));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> Include(Expression<Func<TRootEntity, object>> includeStatement)
    {
        return Include<TRootEntity>(includeStatement);
    }
    
    public IJsonApiQueryClient<TRootEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> orderByStatement) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddOrderByStatement(new SortStatement<TEntity,TRootEntity>(orderByStatement, SortDirection.Ascending));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> OrderBy(Expression<Func<TRootEntity, object>> orderByStatement)
    {
        return OrderBy<TRootEntity>(orderByStatement);
    }
    
    public IJsonApiQueryClient<TRootEntity> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> orderByStatement) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddOrderByStatement(new SortStatement<TEntity,TRootEntity>(orderByStatement, SortDirection.Descending));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> OrderByDescending(Expression<Func<TRootEntity, object>> orderByStatement)
    {
        return OrderByDescending<TRootEntity>(orderByStatement);
    }
    
    public IJsonApiQueryClient<TRootEntity> PageSize<TEntity>(int limit) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddPageSizeStatement(new PageStatement<TEntity,TRootEntity>(limit, PaginationParameter.PageSize));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> PageSize(int limit)
    {
        return PageSize<TRootEntity>(limit);
    }
    
    public IJsonApiQueryClient<TRootEntity> PageNumber<TEntity>(int number) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddPageNumberStatement(new PageStatement<TEntity,TRootEntity>(number, PaginationParameter.PageNumber));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> PageNumber(int number)
    {
        return PageNumber<TRootEntity>(number);
    }

    public async Task<TRootEntity?> FindAsync(object id, CancellationToken cancellationToken = default)
    {
        var responseBody = await MakeCallAsync($"{GetBasePath()}/{id}", cancellationToken);
        var result = JsonConvert.DeserializeObject<TRootEntity>(responseBody, new JsonApiSerializerSettings());
        return result;
    }

    public async Task<TRootEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var responseBody = await MakeCallAsync(GetBasePath(), cancellationToken);
        var result = JsonConvert.DeserializeObject<List<TRootEntity>>(responseBody, new JsonApiSerializerSettings());
        return result?.FirstOrDefault();
    }

    public async Task<List<TRootEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        var responseBody = await MakeCallAsync(GetBasePath(), cancellationToken);
        return JsonConvert.DeserializeObject<List<TRootEntity>>(responseBody, new JsonApiSerializerSettings()) ?? [];
    }

    private static string GetBasePath()
    {
        var rootType = typeof(TRootEntity);
        return $"/{rootType.GetResourceNamespace()}/{rootType.GetResourceName()}";
    }

    private async Task<string> MakeCallAsync(string path, CancellationToken cancellationToken = default)
    {
        var url = _urlBuilder.Build(path);
        using var httpClient = httpClientFactory.CreateClient(typeof(TRootEntity).GetResourceHttpClientId());
        var httpResponse = await httpClient.GetAsync(url, cancellationToken);
        return await httpResponse.Content.ReadAsStringAsync(cancellationToken);
    }
}