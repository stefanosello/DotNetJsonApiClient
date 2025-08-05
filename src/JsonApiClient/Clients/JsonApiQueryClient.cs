using System.Linq.Expressions;
using JsonApiClient.Builders;
using JsonApiClient.Enums;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements;
using JsonApiSerializer;
using Newtonsoft.Json;

namespace JsonApiClient.Clients;

internal class JsonApiQueryClient<TRootEntity>(IHttpClientFactory httpClientFactory, JsonApiClientOptions options) : IJsonApiQueryClient<TRootEntity> where TRootEntity : class, IJsonApiResource
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
    
    public IJsonApiQueryClient<TRootEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> orderByStatement, Expression<Func<TRootEntity, IEnumerable<TEntity>>> subresourceSelector) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddOrderByStatement(new SortStatement<TEntity,TRootEntity>(subresourceSelector, orderByStatement, SortDirection.Ascending));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> OrderBy(Expression<Func<TRootEntity, object>> orderByStatement)
    {
        _urlBuilder.AddOrderByStatement(new SortStatement<TRootEntity,TRootEntity>(null, orderByStatement, SortDirection.Ascending));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> orderByStatement, Expression<Func<TRootEntity, IEnumerable<TEntity>>> subresourceSelector) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddOrderByStatement(new SortStatement<TEntity,TRootEntity>(subresourceSelector, orderByStatement, SortDirection.Descending));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> OrderByDescending(Expression<Func<TRootEntity, object>> orderByStatement)
    {
        _urlBuilder.AddOrderByStatement(new SortStatement<TRootEntity,TRootEntity>(null, orderByStatement, SortDirection.Descending));
        return this;
    }
    
    public IJsonApiQueryClient<TRootEntity> PageSize<TEntity>(int limit, Expression<Func<TRootEntity,IEnumerable<TEntity>>> resourceSelector) where TEntity : class, IJsonApiResource
    {
        if (limit <= 0) throw new ArgumentException("Page size must be greater than 0", nameof(limit));
        return PageSizeInternal(limit, resourceSelector);
    }
    
    public IJsonApiQueryClient<TRootEntity> PageSize(int limit)
    {
        if (limit <= 0) throw new ArgumentException("Page size must be greater than 0", nameof(limit));
        return PageSizeInternal<TRootEntity>(limit);
    }
    
    public IJsonApiQueryClient<TRootEntity> PageNumber<TEntity>(int number, Expression<Func<TRootEntity,IEnumerable<TEntity>>> resourceSelector) where TEntity : class, IJsonApiResource
    {
        if (number <= 0) throw new ArgumentException("Page number must be greater than 0", nameof(number));
        return PageNumberInternal(number, resourceSelector);
    }
    
    public IJsonApiQueryClient<TRootEntity> PageNumber(int number)
    {
        if (number <= 0) throw new ArgumentException("Page number must be greater than 0", nameof(number));
        return PageNumberInternal<TRootEntity>(number);
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
    
    private IJsonApiQueryClient<TRootEntity> PageSizeInternal<TEntity>(int limit, Expression<Func<TRootEntity,IEnumerable<TEntity>>>? resourceSelector = null) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddPageSizeStatement(new PageStatement<TEntity,TRootEntity>(limit, PaginationParameter.PageSize, resourceSelector));
        return this;
    }
    
    private IJsonApiQueryClient<TRootEntity> PageNumberInternal<TEntity>(int limit, Expression<Func<TRootEntity,IEnumerable<TEntity>>>? resourceSelector = null) where TEntity : class, IJsonApiResource
    {
        _urlBuilder.AddPageNumberStatement(new PageStatement<TEntity,TRootEntity>(limit, PaginationParameter.PageNumber, resourceSelector));
        return this;
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
        
        if (options.EnableRetry)
        {
            return await MakeCallWithRetryAsync(httpClient, url, cancellationToken);
        }
        
        var httpResponse = await httpClient.GetAsync(url, cancellationToken);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new JsonApiHttpException((int)httpResponse.StatusCode, 
                $"HTTP request failed with status code {httpResponse.StatusCode}", content);
        }
        return await httpResponse.Content.ReadAsStringAsync(cancellationToken);
    }
    
    private async Task<string> MakeCallWithRetryAsync(HttpClient httpClient, string url, CancellationToken cancellationToken)
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                var httpResponse = await httpClient.GetAsync(url, cancellationToken);
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    return await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                }
                
                var statusCode = (int)httpResponse.StatusCode;
                var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                
                // If not retryable or max retries reached, throw our custom exception
                if (!options.RetryableStatusCodes.Contains(statusCode) || 
                    attempt >= options.MaxRetries)
                {
                    throw new JsonApiHttpException(statusCode, 
                        $"HTTP request failed with status code {httpResponse.StatusCode}", content);
                }
                
                attempt++;
                if (attempt <= options.MaxRetries)
                {
                    await Task.Delay(options.RetryDelay, cancellationToken);
                    continue;
                }
                
                // Should not reach here, but just in case
                throw new JsonApiHttpException(statusCode, 
                    $"HTTP request failed with status code {httpResponse.StatusCode}", content);
            }
            catch (HttpRequestException) when (attempt < options.MaxRetries)
            {
                attempt++;
                if (attempt <= options.MaxRetries)
                {
                    await Task.Delay(options.RetryDelay, cancellationToken);
                    continue;
                }
                throw;
            }
        }
    }
}