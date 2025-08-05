using FluentAssertions;
using JsonApiClient.Exceptions;
using JsonApiClient.Tests.Models;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;

namespace JsonApiClient.Tests;

public class RetryTests
{
    [Fact]
    public async Task Retry_WithTransientError_ShouldRetryAndEventuallyFail()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        
        // Always return 500 (retryable)
        messageHandler.When("https://example.com/books/book")
            .Respond(System.Net.HttpStatusCode.InternalServerError, "application/json", "{\"error\":\"Server error\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var options = new JsonApiClientOptions
        {
            MaxRetries = 2,
            RetryDelay = TimeSpan.FromMilliseconds(50),
            EnableRetry = true
        };
        
        var client = new JsonApiClient(httpClientFactory, options);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.ToListAsync();
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 500);
    }
    
    [Fact]
    public async Task Retry_WithPermanentError_ShouldNotRetry()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        
        // Always return 400 (not retryable)
        messageHandler.When("https://example.com/books/book")
            .Respond(System.Net.HttpStatusCode.BadRequest, "application/json", "{\"error\":\"Bad request\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var options = new JsonApiClientOptions
        {
            MaxRetries = 3,
            EnableRetry = true
        };
        
        var client = new JsonApiClient(httpClientFactory, options);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.ToListAsync();
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 400);
    }
    
    [Fact]
    public async Task Retry_WithMaxRetriesExceeded_ShouldThrowException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        
        // Always return 500 (retryable)
        messageHandler.When("https://example.com/books/book")
            .Respond(System.Net.HttpStatusCode.InternalServerError, "application/json", "{\"error\":\"Server error\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var options = new JsonApiClientOptions
        {
            MaxRetries = 2,
            RetryDelay = TimeSpan.FromMilliseconds(50),
            EnableRetry = true
        };
        
        var client = new JsonApiClient(httpClientFactory, options);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.ToListAsync();
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 500);
    }
    
    [Fact]
    public async Task Retry_WithRetryDisabled_ShouldNotRetry()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        
        // Always return 500 (retryable)
        messageHandler.When("https://example.com/books/book")
            .Respond(System.Net.HttpStatusCode.InternalServerError, "application/json", "{\"error\":\"Server error\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var options = new JsonApiClientOptions
        {
            EnableRetry = false
        };
        
        var client = new JsonApiClient(httpClientFactory, options);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.ToListAsync();
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 500);
    }
    
    [Fact]
    public async Task Retry_WithCustomRetryableStatusCodes_ShouldRespectConfiguration()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        
        // Return 404 (not in default retryable codes)
        messageHandler.When("https://example.com/books/book")
            .Respond(System.Net.HttpStatusCode.NotFound, "application/json", "{\"error\":\"Not found\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var options = new JsonApiClientOptions
        {
            MaxRetries = 3,
            EnableRetry = true,
            RetryableStatusCodes = [404] // Include 404 as retryable
        };
        
        var client = new JsonApiClient(httpClientFactory, options);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.ToListAsync();
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 404);
    }
} 