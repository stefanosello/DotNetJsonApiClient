using System.Linq.Expressions;
using JsonApiClient.Enums;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements;

namespace JsonApiClient;

public class JsonApiClientBuilder<TRootEntity>(string baseUrl) : IJsonApiClientBuilder<TRootEntity> where TRootEntity : class
{
    private IHttpClientFactory? _httpClientFactory = null;
    private string? _httpClientId = null;
    private readonly ICollection<IStatement> _statements = [];

    public IJsonApiClientBuilder<TRootEntity> SetHttpClient(IHttpClientFactory httpClientFactory, string httpClientId)
    {
        _httpClientFactory = httpClientFactory;
        _httpClientId = httpClientId;
        return this;
    }

    public IJsonApiClientBuilder<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement) where TEntity : class
    {
        _statements.Add(new SelectStatement<TEntity>(selectStatement));
        return this;
    }
    
    public IJsonApiClientBuilder<TRootEntity> Select(Expression<Func<TRootEntity, object>> selectStatement)
    {
        return Select<TRootEntity>(selectStatement);
    }
    
    public IJsonApiClientBuilder<TRootEntity> Where<TEntity>(Expression<Func<TEntity, bool>> whereStatement) where TEntity : class
    {
        _statements.Add(new WhereStatement<TEntity>(whereStatement));
        return this;
    }
    
    public IJsonApiClientBuilder<TRootEntity> Where(Expression<Func<TRootEntity, bool>> whereStatement)
    {
        return Where<TRootEntity>(whereStatement);
    }
    
    public IJsonApiClientBuilder<TRootEntity> Include<TEntity>(Expression<Func<TEntity, object>> includeStatement) where TEntity : class
    {
        _statements.Add(new IncludeStatement<TEntity>(includeStatement));
        return this;
    }
    
    public IJsonApiClientBuilder<TRootEntity> Include(Expression<Func<TRootEntity, object>> includeStatement)
    {
        return Include<TRootEntity>(includeStatement);
    }
    
    public IJsonApiClientBuilder<TRootEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> orderByStatement) where TEntity : class
    {
        _statements.Add(new SortStatement<TEntity>(orderByStatement, SortDirection.Ascending));
        return this;
    }
    
    public IJsonApiClientBuilder<TRootEntity> OrderBy(Expression<Func<TRootEntity, object>> orderByStatement)
    {
        return OrderBy<TRootEntity>(orderByStatement);
    }
    
    public IJsonApiClientBuilder<TRootEntity> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> orderByStatement) where TEntity : class
    {
        _statements.Add(new SortStatement<TEntity>(orderByStatement, SortDirection.Descending));
        return this;
    }
    
    public IJsonApiClientBuilder<TRootEntity> OrderByDescending(Expression<Func<TRootEntity, object>> orderByStatement)
    {
        return OrderByDescending<TRootEntity>(orderByStatement);
    }
    
    public IJsonApiClientBuilder<TRootEntity> PageSize<TEntity>(int limit) where TEntity : class
    {
        _statements.Add(new PageStatement<TEntity>(limit, PaginationParameter.PageSize));
        return this;
    }
    
    public IJsonApiClientBuilder<TRootEntity> PageSize(int limit)
    {
        return PageSize<TRootEntity>(limit);
    }
    
    public IJsonApiClientBuilder<TRootEntity> PageNumber<TEntity>(int number) where TEntity : class
    {
        _statements.Add(new PageStatement<TEntity>(number, PaginationParameter.PageNumber));
        return this;
    }
    
    public IJsonApiClientBuilder<TRootEntity> PageNumber(int number)
    {
        return PageNumber<TRootEntity>(number);
    }

    public IJsonApiClient<TRootEntity> Build()
    {
        if (_httpClientId == null || _httpClientFactory == null)
            throw new InvalidOperationException("HttpClientFactory and HttpClientId cannot be null.");
        
        return new JsonApiClient<TRootEntity>(_httpClientFactory, _httpClientId, BuildUrl());
    }

    private string BuildUrl()
    {
        UriBuilder urlBuilder = new(baseUrl);
        var path = GetPath();
        urlBuilder.Path = path;
        
        foreach (var statement in _statements)
            statement.Validate();
        
        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        foreach (var statement in _statements)
        {
            string? targetResourceName = null;
            if (statement.GetType().GetGenericArguments().First() != typeof(TRootEntity))
                targetResourceName = statement.GetType().GetGenericArguments().First().GetResourceName();
            
            var param = statement.Translate(targetResourceName);
            queryString.Add(param.Key, param.Value);
        }
        urlBuilder.Query = queryString.ToString();

        return urlBuilder.ToString();
    }

    private static string GetPath()
    {
        var rootType = typeof(TRootEntity);
        return $"/{rootType.GetResourceNamespace()}/{rootType.GetResourceName()}";
    }
}