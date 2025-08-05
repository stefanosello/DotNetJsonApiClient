using FluentAssertions;
using JsonApiClient.Exceptions;
using JsonApiClient.Tests.Models;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;

namespace JsonApiClient.Tests;

public class ErrorHandlingTests
{
    [Fact]
    public async Task FindAsync_WithHttpError_ShouldThrowJsonApiHttpException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        messageHandler.When("https://example.com/books/book/1")
            .Respond(System.Net.HttpStatusCode.NotFound, "application/json", "{\"error\":\"Not found\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.FindAsync(1);
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 404 && ex.ResponseContent == "{\"error\":\"Not found\"}");
    }
    
    [Fact]
    public async Task ToListAsync_WithHttpError_ShouldThrowJsonApiHttpException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        messageHandler.When("https://example.com/books/book")
            .Respond(System.Net.HttpStatusCode.InternalServerError, "application/json", "{\"error\":\"Server error\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.ToListAsync();
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 500 && ex.ResponseContent == "{\"error\":\"Server error\"}");
    }
    
    [Fact]
    public async Task FirstOrDefaultAsync_WithHttpError_ShouldThrowJsonApiHttpException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var messageHandler = new MockHttpMessageHandler();
        messageHandler.When("https://example.com/books/book")
            .Respond(System.Net.HttpStatusCode.BadRequest, "application/json", "{\"error\":\"Bad request\"}");
        
        var httpClient = new HttpClient(messageHandler) { BaseAddress = new Uri("https://example.com") };
        httpClientFactory.CreateClient("api.books").Returns(httpClient);
        
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = async () => await query.FirstOrDefaultAsync();
        await action.Should().ThrowAsync<JsonApiHttpException>()
            .Where(ex => ex.StatusCode == 400 && ex.ResponseContent == "{\"error\":\"Bad request\"}");
    }
    

    
    [Fact]
    public void JsonApiClient_WithNullOptions_ShouldUseDefaultOptions()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        
        // Act
        var client = new JsonApiClient(httpClientFactory, null);
        
        // Assert
        client.Should().NotBeNull();
    }
    
    [Fact]
    public void JsonApiClient_WithCustomOptions_ShouldUseCustomOptions()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var customOptions = new JsonApiClientOptions
        {
            MaxRetries = 5,
            EnableRetry = false
        };
        
        // Act
        var client = new JsonApiClient(httpClientFactory, customOptions);
        
        // Assert
        client.Should().NotBeNull();
    }
} 