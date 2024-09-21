using System.Collections.Specialized;
using System.Linq.Expressions;
using JsonApiClient.Attributes;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements;

namespace JsonApiClient;

public class JsonApiClientBuilder<TRootEntity>(string baseUrl) : IJsonApiClientBuilder<TRootEntity> where TRootEntity : class
{
    private IHttpClientFactory? _httpClientFactory = null;
    private string? _httpClientId = null;
    private ICollection<IStatement> _statements = [];

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
    
    public IJsonApiClientBuilder<TRootEntity> Where<TEntity>(Expression<Func<TEntity, bool>> whereStatement) where TEntity : class
    {
        _statements.Add(new WhereStatement<TEntity>(whereStatement));
        return this;
    }

    public IJsonApiClient<TRootEntity> Build()
    {
        // TODO: validation before building
        if (_httpClientId == null || _httpClientFactory == null)
            throw new InvalidOperationException("HttpClientFactory and HttpClientId cannot be null.");
        return new JsonApiClient<TRootEntity>(_httpClientFactory, _httpClientId, BuildUrl());
    }

    private string BuildUrl()
    {
        UriBuilder urlBuilder = new(baseUrl);
        var path = GetPath();
        urlBuilder.Path = path;
        
        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        foreach (var statement in _statements)
        {
            KeyValuePair<string, string> param = statement.Translate();
            queryString.Add(param.Key, param.Value);
        }
        urlBuilder.Query = queryString.ToString();

        return urlBuilder.ToString();
    }

    private string GetPath()
    {
        var rootType = typeof(TRootEntity);
        var attribute =
            (JResAttribute?)Attribute.GetCustomAttribute(rootType, typeof(JResAttribute)) ??
            throw new MissingAttributeException($"The provided root type is not decorated with the {nameof(JResAttribute)} attribute.");
        return $"/{attribute.ApiNamespace}/{attribute.ResourceName ?? rootType.Name.Uncapitalize()}";
    }
}