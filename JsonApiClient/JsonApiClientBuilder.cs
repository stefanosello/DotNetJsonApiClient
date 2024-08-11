using System.Collections.Specialized;
using System.Linq.Expressions;
using JsonApiClient.Attributes;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Resources;

namespace JsonApiClient;

public class JsonApiClientBuilder<TRootEntity>(string baseUrl) : IJsonApiClientBuilder<TRootEntity> where TRootEntity : class, IIdentifiable
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

    public IJsonApiClientBuilder<TRootEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectStatement) where TEntity : class, IIdentifiable
    {
        _statements.Add(new SelectStatement<TEntity>(selectStatement));
        return this;
    }

    public IJsonApiClient<TRootEntity> Build()
    {
        // TODO: validation before building
        if (_httpClientId == null || _httpClientFactory == null)
            throw new InvalidConfigurationException("HttpClientFactory and HttpClientId cannot be null.");
        return new JsonApiClient<TRootEntity>(_httpClientFactory, _httpClientId, BuildUrl());
    }

    private string BuildUrl()
    {
        UriBuilder urlBuilder = new(baseUrl);
        string path = GetPath();
        urlBuilder.Path = path;
        
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        foreach (var statement in _statements)
        {
            KeyValuePair<string, string> param = statement.Parse();
            queryString.Add(param.Key, param.Value);
        }
        urlBuilder.Query = queryString.ToString();

        return urlBuilder.ToString();
    }

    private string GetPath()
    {
        Type rootType = typeof(TRootEntity);
        JsonApiEntityAttribute? attribute = (JsonApiEntityAttribute?) Attribute.GetCustomAttribute(rootType, typeof(JsonApiEntityAttribute));
        return attribute is null ? $"{rootType.Name.Uncapitalize()}/{rootType.Name.Uncapitalize()}" : $"/{attribute.JsonApiNamespace}/{attribute.JsonApiResource ?? rootType.Name.Uncapitalize()}";
    }
}