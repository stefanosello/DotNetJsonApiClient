using FluentAssertions;
using Xunit;

namespace JsonApiClient.Tests;

public class JsonApiClientOptionsTests
{
    [Fact]
    public void DefaultOptions_ShouldHaveCorrectDefaultValues()
    {
        // Act
        var options = new JsonApiClientOptions();
        
        // Assert
        options.MaxRetries.Should().Be(3);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(1));
        options.RequestTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.EnableRetry.Should().BeTrue();
        options.RetryableStatusCodes.Should().BeEquivalentTo([408, 429, 500, 502, 503, 504]);
    }
    
    [Fact]
    public void CustomOptions_ShouldSetCorrectValues()
    {
        // Arrange
        var customRetryCodes = new[] { 500, 502 };
        
        // Act
        var options = new JsonApiClientOptions
        {
            MaxRetries = 5,
            RetryDelay = TimeSpan.FromSeconds(2),
            RequestTimeout = TimeSpan.FromSeconds(60),
            EnableRetry = false,
            RetryableStatusCodes = customRetryCodes
        };
        
        // Assert
        options.MaxRetries.Should().Be(5);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(2));
        options.RequestTimeout.Should().Be(TimeSpan.FromSeconds(60));
        options.EnableRetry.Should().BeFalse();
        options.RetryableStatusCodes.Should().BeEquivalentTo(customRetryCodes);
    }
} 