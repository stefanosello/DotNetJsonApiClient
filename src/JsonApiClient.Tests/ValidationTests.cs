using FluentAssertions;
using JsonApiClient.Tests.Models;
using NSubstitute;
using Xunit;

namespace JsonApiClient.Tests;

public class ValidationTests
{
    [Fact]
    public void PageSize_WithZero_ShouldThrowArgumentException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageSize(0);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Page size must be greater than 0*")
            .And.ParamName.Should().Be("limit");
    }
    
    [Fact]
    public void PageSize_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageSize(-5);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Page size must be greater than 0*")
            .And.ParamName.Should().Be("limit");
    }
    
    [Fact]
    public void PageSize_WithValidValue_ShouldNotThrow()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageSize(10);
        action.Should().NotThrow();
    }
    
    [Fact]
    public void PageNumber_WithZero_ShouldThrowArgumentException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageNumber(0);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Page number must be greater than 0*")
            .And.ParamName.Should().Be("number");
    }
    
    [Fact]
    public void PageNumber_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageNumber(-1);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Page number must be greater than 0*")
            .And.ParamName.Should().Be("number");
    }
    
    [Fact]
    public void PageNumber_WithValidValue_ShouldNotThrow()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageNumber(1);
        action.Should().NotThrow();
    }
    
    [Fact]
    public void PageSize_WithSubresource_WithZero_ShouldThrowArgumentException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageSize(0, b => b.TagsBooks);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Page size must be greater than 0*")
            .And.ParamName.Should().Be("limit");
    }
    
    [Fact]
    public void PageNumber_WithSubresource_WithZero_ShouldThrowArgumentException()
    {
        // Arrange
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var client = new JsonApiClient(httpClientFactory);
        var query = client.Query<Book>();
        
        // Act & Assert
        var action = () => query.PageNumber(0, b => b.TagsBooks);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Page number must be greater than 0*")
            .And.ParamName.Should().Be("number");
    }
} 