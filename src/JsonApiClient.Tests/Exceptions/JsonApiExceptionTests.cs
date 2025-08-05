using FluentAssertions;
using JsonApiClient.Exceptions;
using Xunit;

namespace JsonApiClient.Tests.Exceptions;

public class JsonApiExceptionTests
{
    [Fact]
    public void JsonApiException_WithMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Test error message";
        
        // Act
        var exception = new JsonApiException(message);
        
        // Assert
        exception.Message.Should().Be(message);
    }
    
    [Fact]
    public void JsonApiException_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        const string message = "Test error message";
        var innerException = new InvalidOperationException("Inner error");
        
        // Act
        var exception = new JsonApiException(message, innerException);
        
        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}

public class JsonApiHttpExceptionTests
{
    [Fact]
    public void JsonApiHttpException_WithStatusCodeAndMessage_ShouldSetProperties()
    {
        // Arrange
        const int statusCode = 404;
        const string message = "Resource not found";
        const string responseContent = "{\"error\":\"Not found\"}";
        
        // Act
        var exception = new JsonApiHttpException(statusCode, message, responseContent);
        
        // Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(message);
        exception.ResponseContent.Should().Be(responseContent);
    }
    
    [Fact]
    public void JsonApiHttpException_WithInnerException_ShouldSetInnerException()
    {
        // Arrange
        const int statusCode = 500;
        const string message = "Server error";
        var innerException = new HttpRequestException("Network error");
        
        // Act
        var exception = new JsonApiHttpException(statusCode, message, innerException);
        
        // Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
    
    [Fact]
    public void JsonApiHttpException_WithoutResponseContent_ShouldSetNull()
    {
        // Arrange
        const int statusCode = 400;
        const string message = "Bad request";
        
        // Act
        var exception = new JsonApiHttpException(statusCode, message);
        
        // Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(message);
        exception.ResponseContent.Should().BeNull();
    }
} 