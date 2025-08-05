using FluentAssertions;
using JsonApiClient.Extensions;
using JsonApiClient.Tests.Models;
using NSubstitute;
using System.Diagnostics;
using Xunit;

namespace JsonApiClient.Tests;

public class PerformanceTests
{
    [Fact]
    public void GetResourceName_Cached_ShouldBeFasterOnSecondCall()
    {
        // Arrange
        var type = typeof(Book);
        
        // Act - First call (cache miss)
        var stopwatch1 = Stopwatch.StartNew();
        var result1 = type.GetResourceName();
        stopwatch1.Stop();
        
        // Act - Second call (cache hit)
        var stopwatch2 = Stopwatch.StartNew();
        var result2 = type.GetResourceName();
        stopwatch2.Stop();
        
        // Assert
        result1.Should().Be(result2);
        stopwatch2.ElapsedTicks.Should().BeLessThan(stopwatch1.ElapsedTicks);
    }
    
    [Fact]
    public void GetResourceHttpClientId_Cached_ShouldBeFasterOnSecondCall()
    {
        // Arrange
        var type = typeof(Book);
        
        // Act - First call (cache miss)
        var stopwatch1 = Stopwatch.StartNew();
        var result1 = type.GetResourceHttpClientId();
        stopwatch1.Stop();
        
        // Act - Second call (cache hit)
        var stopwatch2 = Stopwatch.StartNew();
        var result2 = type.GetResourceHttpClientId();
        stopwatch2.Stop();
        
        // Assert
        result1.Should().Be(result2);
        stopwatch2.ElapsedTicks.Should().BeLessThan(stopwatch1.ElapsedTicks);
    }
    
    [Fact]
    public void GetResourceNamespace_Cached_ShouldBeFasterOnSecondCall()
    {
        // Arrange
        var type = typeof(Book);
        
        // Act - First call (cache miss)
        var stopwatch1 = Stopwatch.StartNew();
        var result1 = type.GetResourceNamespace();
        stopwatch1.Stop();
        
        // Act - Second call (cache hit)
        var stopwatch2 = Stopwatch.StartNew();
        var result2 = type.GetResourceNamespace();
        stopwatch2.Stop();
        
        // Assert
        result1.Should().Be(result2);
        stopwatch2.ElapsedTicks.Should().BeLessThan(stopwatch1.ElapsedTicks);
    }
    
    [Fact]
    public void MultipleResourceTypes_ShouldCacheIndependently()
    {
        // Arrange
        var bookType = typeof(Book);
        var authorType = typeof(Author);
        
        // Act
        var bookResourceName = bookType.GetResourceName();
        var authorResourceName = authorType.GetResourceName();
        var bookResourceName2 = bookType.GetResourceName();
        var authorResourceName2 = authorType.GetResourceName();
        
        // Assert
        bookResourceName.Should().Be(bookResourceName2);
        authorResourceName.Should().Be(authorResourceName2);
        bookResourceName.Should().NotBe(authorResourceName);
    }
    
    [Fact]
    public async Task ConcurrentAccess_ShouldNotThrowExceptions()
    {
        // Arrange
        var type = typeof(Book);
        var tasks = new List<Task<string>>();
        
        // Act
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() => type.GetResourceName()));
        }
        
        var results = await Task.WhenAll(tasks);
        
        // Assert
        results.Should().AllBeEquivalentTo(results[0]);
    }
} 