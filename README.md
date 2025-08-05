![logo.svg](https://raw.githubusercontent.com/stefanosello/DotNetJsonApiClient/refs/heads/main/logo.svg)

[![codecov](https://codecov.io/gh/stefanosello/DotNetJsonApiClient/graph/badge.svg?token=VGZAWJNI5X)](https://codecov.io/gh/stefanosello/DotNetJsonApiClient)
[![PUBLISH](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/publish.yml/badge.svg)](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/publish.yml)
[![DOCS](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/docs.yml/badge.svg)](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/docs.yml)
[![NuGet Version](https://img.shields.io/nuget/v/DotNetJsonApiClient)](https://www.nuget.org/packages/DotNetJsonApiClient#versions-body-tab)
[![Open Source? Yes!](https://badgen.net/badge/Open%20Source%20%3F/Yes%21/blue?icon=github)](https://github.com/Naereen/badges/)

# DotNetJsonApiClient

A lightweight `.NET 8` client library designed for APIs adhering to the `json:api` standard. It offers an EF Core-like experience for querying data from `json:api` endpoints, simplifying integration and development workflows.

## 🎯 Purpose

This library was born out of the need for an easy-to-use solution to fetch data from `HTTP` endpoints in a microservice architecture, where some services act as data providers and others as consumers. Its intuitive design makes it an excellent companion to the [JsonApiDotNetCore](https://www.jsonapi.net/index.html) library, as both adhere to the same standard, ensuring seamless interoperability.

## ✨ Features

- **EF Core-like Query Experience** - Fluent API for building complex queries
- **Type-Safe Operations** - Compile-time safety with strongly-typed models
- **Comprehensive Error Handling** - Custom exceptions with detailed error information
- **Retry Policies** - Configurable retry logic for transient failures
- **Performance Optimized** - Caching for reflection results and efficient HTTP handling
- **Input Validation** - Built-in validation for pagination and query parameters
- **Null Safety** - Comprehensive null handling throughout the codebase

## 📦 Installation

### NuGet Package

```bash
dotnet add package DotNetJsonApiClient
```

### Package Manager

```powershell
Install-Package DotNetJsonApiClient
```

## 🚀 Quick Start

### 1. Define Your Models

```csharp
using JsonApiClient.Attributes;
using JsonApiClient.Models;

[JRes("api.books", "api", "book")]
public class Book : JResource<int>
{
    [JAttr]
    public string? Title { get; set; }
    
    [JAttr]
    public DateTime? PublishDate { get; set; }
    
    [JAttr]
    public bool Deleted { get; set; }
    
    [JRel]
    public Author? Author { get; set; }
}

[JRes("api.books", "api", "author")]
public class Author : JResource<int>
{
    [JAttr]
    public string? FirstName { get; set; }
    
    [JAttr]
    public string? LastName { get; set; }
    
    [JRel]
    public ICollection<Book> Books { get; set; } = [];
}
```

### 2. Configure HTTP Client

```csharp
// Program.cs or Startup.cs
builder.Services.AddHttpClient("api.books", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.api+json");
});
```

### 3. Use the Client

```csharp
// Basic usage
var client = new JsonApiClient(httpClientFactory);
var books = await client.Query<Book>().ToListAsync();

// With filtering
var filteredBooks = await client.Query<Book>()
    .Where(b => b.Title == "The Hobbit" && !b.Deleted)
    .ToListAsync();

// With includes and pagination
var booksWithAuthors = await client.Query<Book>()
    .Include(b => b.Author)
    .Select(b => new { b.Title, b.Author })
    .PageSize(10)
    .PageNumber(1)
    .ToListAsync();
```

## 📚 Detailed Usage

### Basic Querying

```csharp
// Get all books
var allBooks = await client.Query<Book>().ToListAsync();

// Get a specific book by ID
var book = await client.Query<Book>().FindAsync(1);

// Get first book matching criteria
var firstBook = await client.Query<Book>()
    .Where(b => b.Title.Contains("Hobbit"))
    .FirstOrDefaultAsync();
```

### Filtering

```csharp
// Simple equality
var books = await client.Query<Book>()
    .Where(b => b.Title == "The Hobbit")
    .ToListAsync();

// Complex conditions
var books = await client.Query<Book>()
    .Where(b => b.Title.Contains("Hobbit") && !b.Deleted)
    .ToListAsync();

// Date comparisons
var recentBooks = await client.Query<Book>()
    .Where(b => b.PublishDate >= DateTime.Today.AddYears(-1))
    .ToListAsync();

// String operations
var books = await client.Query<Book>()
    .Where(b => b.Title.StartsWith("The") || b.Title.EndsWith("Ring"))
    .ToListAsync();
```

### Selecting Fields

```csharp
// Select specific fields
var books = await client.Query<Book>()
    .Select(b => new { b.Title, b.PublishDate })
    .ToListAsync();

// Select fields for related resources
var books = await client.Query<Book>()
    .Select(b => new { b.Title })
    .Select<Author>(a => new { a.FirstName, a.LastName })
    .Include(b => b.Author)
    .ToListAsync();
```

### Including Related Resources

```csharp
// Include single relationship
var books = await client.Query<Book>()
    .Include(b => b.Author)
    .ToListAsync();

// Include multiple relationships
var books = await client.Query<Book>()
    .Include(b => b.Author)
    .Include(b => b.Tags)
    .ToListAsync();
```

### Sorting

```csharp
// Ascending order
var books = await client.Query<Book>()
    .OrderBy(b => b.Title)
    .ToListAsync();

// Descending order
var books = await client.Query<Book>()
    .OrderByDescending(b => b.PublishDate)
    .ToListAsync();

// Sort related resources
var authors = await client.Query<Author>()
    .OrderBy(b => b.Title, a => a.Books)
    .ToListAsync();
```

### Pagination

```csharp
// Basic pagination
var books = await client.Query<Book>()
    .PageSize(10)
    .PageNumber(2)
    .ToListAsync();

// Paginate related resources
var authors = await client.Query<Author>()
    .PageSize(5, a => a.Books)
    .PageNumber(1, a => a.Books)
    .ToListAsync();
```

### Complex Queries

```csharp
var books = await client.Query<Book>()
    .Select(b => new { b.Title, b.Author })
    .Select<Author>(a => new { a.FirstName, a.LastName })
    .Where(b => b.Title.Contains("Ring") && !b.Deleted)
    .Include(b => b.Author)
    .OrderBy(b => b.Title)
    .PageSize(20)
    .PageNumber(1)
    .ToListAsync();
```

## ⚙️ Configuration

### Client Options

```csharp
var options = new JsonApiClientOptions
{
    MaxRetries = 3,
    RetryDelay = TimeSpan.FromSeconds(1),
    RequestTimeout = TimeSpan.FromSeconds(30),
    EnableRetry = true,
    RetryableStatusCodes = [408, 429, 500, 502, 503, 504]
};

var client = new JsonApiClient(httpClientFactory, options);
```

### Dependency Injection

```csharp
// Register in DI container
builder.Services.AddSingleton<JsonApiClientOptions>(options);
builder.Services.AddScoped<JsonApiClient>();

// Use in your services
public class BookService
{
    private readonly JsonApiClient _client;
    
    public BookService(JsonApiClient client)
    {
        _client = client;
    }
    
    public async Task<List<Book>> GetBooksAsync()
    {
        return await _client.Query<Book>().ToListAsync();
    }
}
```

## 🛡️ Error Handling

### Custom Exceptions

```csharp
try
{
    var books = await client.Query<Book>().ToListAsync();
}
catch (JsonApiHttpException ex)
{
    Console.WriteLine($"HTTP Error: {ex.StatusCode}");
    Console.WriteLine($"Response: {ex.ResponseContent}");
}
catch (JsonApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
}
```

### Validation Errors

```csharp
try
{
    var books = await client.Query<Book>()
        .PageSize(0) // This will throw ArgumentException
        .ToListAsync();
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation Error: {ex.Message}");
}
```

## 📊 Performance Features

### Caching

The library automatically caches reflection results for better performance:

```csharp
// First call - cache miss (slower)
var resourceName = typeof(Book).GetResourceName();

// Subsequent calls - cache hit (faster)
var resourceName2 = typeof(Book).GetResourceName();
```

### Retry Logic

Automatic retry for transient failures:

```csharp
var options = new JsonApiClientOptions
{
    MaxRetries = 3,
    RetryDelay = TimeSpan.FromMilliseconds(100),
    EnableRetry = true
};

var client = new JsonApiClient(httpClientFactory, options);
// Will automatically retry on 500, 502, 503, 504 errors
```

## 🔧 Advanced Usage

### Custom HTTP Headers

```csharp
builder.Services.AddHttpClient("api.books", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.api+json");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
});
```

### Multiple API Endpoints

```csharp
// Configure multiple named HTTP clients
builder.Services.AddHttpClient("api.books", client =>
{
    client.BaseAddress = new Uri("https://books-api.example.com/");
});

builder.Services.AddHttpClient("api.authors", client =>
{
    client.BaseAddress = new Uri("https://authors-api.example.com/");
});

// Use different clients for different resources
var booksClient = new JsonApiClient(httpClientFactory);
var authorsClient = new JsonApiClient(httpClientFactory);
```

## 🧪 Testing

### Unit Testing with Mocks

```csharp
[Fact]
public async Task GetBooks_ShouldReturnBooks()
{
    // Arrange
    var httpClientFactory = Substitute.For<IHttpClientFactory>();
    var messageHandler = new MockHttpMessageHandler();
    messageHandler.When("https://api.example.com/api/book")
        .Respond("application/vnd.api+json", jsonResponse);
    
    var httpClient = new HttpClient(messageHandler);
    httpClientFactory.CreateClient("api.books").Returns(httpClient);
    
    var client = new JsonApiClient(httpClientFactory);
    
    // Act
    var books = await client.Query<Book>().ToListAsync();
    
    // Assert
    books.Should().HaveCount(1);
    books[0].Title.Should().Be("The Hobbit");
}
```

## 📋 Requirements

- .NET 8.0 or later
- HTTP client configured with named client
- Models decorated with appropriate attributes

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/DotNetJsonApiClient)
- [Documentation](https://stefanosello.github.io/DotNetJsonApiClient/)
- [JSON:API Specification](https://jsonapi.org/)
- [JsonApiDotNetCore](https://www.jsonapi.net/index.html)

## 🆘 Support

- [GitHub Issues](https://github.com/stefanosello/DotNetJsonApiClient/issues)
- [GitHub Discussions](https://github.com/stefanosello/DotNetJsonApiClient/discussions)
- [Documentation](https://stefanosello.github.io/DotNetJsonApiClient/)
