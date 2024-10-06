using FluentAssertions;
using JsonApiClient.Tests.Models;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace JsonApiClient.Tests;

public class JsonApiClientTests
{
    [Fact]
    public async Task Test1()
    {
      const string responseData = """
                                    {
                                      "links": {
                                        "self": "https://example.jsonapi.com/books",
                                        "next": "http://https://example.jsonapi.com/books?page[offset]=2",
                                        "last": "http://https://example.jsonapi.com/books?page[offset]=10"
                                      },
                                      "data": [{
                                        "type": "book",
                                        "id": "1",
                                        "attributes": {
                                          "title": "Dracula"
                                        },
                                        "links": {
                                          "self": "https://example.jsonapi.com/books/1"
                                        }
                                      }]
                                    }
                                  """;
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient("api.books").Returns(GetMockedHttpClient("https://example.jsonapi.com", "/books/book?fields[book]=title", responseData));
        var sut = new JsonApiClient<Book>(httpClientFactory).Query;

        sut.Select<Book>(x => new
        {
            x.Title
        });

        var response = await sut.ToListAsync();
        response.Should().BeEquivalentTo([new Book() { Title = "Dracula", Id = 1 }]);
    }
    
    [Fact]
    public async Task Test2()
    {
        const string responseData = """
                                      {
                                        "links": {
                                          "self": "https://example.jsonapi.com/books",
                                          "next": "http://https://example.jsonapi.com/books?page[offset]=2",
                                          "last": "http://https://example.jsonapi.com/books?page[offset]=10"
                                        },
                                        "data": [{
                                          "type": "book",
                                          "id": "1",
                                          "attributes": {
                                            "title": "Dracula"
                                          },
                                          "links": {
                                            "self": "https://example.jsonapi.com/books/1"
                                          }
                                        }]
                                      }
                                    """;
        
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient("api.books").Returns(GetMockedHttpClient("https://example.jsonapi.com", "/books/book?fields[book]=title&filter=and(equals(title,'Dracula'),not(equals(deleted,'true')))", responseData));
        var sut = new JsonApiClient<Book>(httpClientFactory).Query;

        sut
          .Select<Book>(x => new
          {
            x.Title
          })
          .Where<Book>(x => x.Title == "Dracula" && !x.Deleted);

        var response = await sut.FirstOrDefaultAsync();
        response.Should().BeEquivalentTo(new Book() { Title = "Dracula", Id = 1 });
    }
    
    [Fact]
    public async Task Test3()
    {
      const string responseData = """
                                    {
                                      "links": {
                                        "self": "https://example.jsonapi.com/books",
                                        "next": "http://https://example.jsonapi.com/books?page[offset]=2",
                                        "last": "http://https://example.jsonapi.com/books?page[offset]=10"
                                      },
                                      "data": [{
                                        "type": "book",
                                        "id": "1",
                                        "attributes": {
                                          "title": "Dracula"
                                        },
                                        "links": {
                                          "self": "https://example.jsonapi.com/books/1"
                                        }
                                      }]
                                    }
                                  """;
      
      var httpClientFactory = Substitute.For<IHttpClientFactory>();
      httpClientFactory.CreateClient("api.books").Returns(GetMockedHttpClient("https://example.jsonapi.com", "/books/book?filter=and(equals(title,'Dracula'),equals(deleted,'true'))&fields[book]=title", responseData));
      var sut = new JsonApiClient<Book>(httpClientFactory).Query;

      sut
        .Select<Book>(x => new
        {
          x.Title
        })
        .Where<Book>(x => x.Title == "Dracula" && x.Deleted);

      var response = await sut.FirstOrDefaultAsync();
      response.Should().BeEquivalentTo(new Book() { Title = "Dracula", Id = 1 });
    }
    
    [Fact]
    public async Task Test4()
    {
      const string responseData = """
                                    {
                                      "links": {
                                        "self": "https://example.jsonapi.com/books",
                                        "next": "http://https://example.jsonapi.com/books?page[offset]=2",
                                        "last": "http://https://example.jsonapi.com/books?page[offset]=10"
                                      },
                                      "data": [{
                                        "type": "book",
                                        "id": "1",
                                        "attributes": {
                                          "title": "Dracula"
                                        },
                                        "links": {
                                          "self": "https://example.jsonapi.com/books/1"
                                        }
                                      }]
                                    }
                                  """;
      
      var httpClientFactory = Substitute.For<IHttpClientFactory>();
      httpClientFactory.CreateClient("api.books").Returns(GetMockedHttpClient("https://example.jsonapi.com", "/books/book?include=author&filter=and(equals(title,'Dracula'),equals(deleted,'true'))&fields[book]=title&sort=title&page[size]=10&page[number]=2", responseData));
      var sut = new JsonApiClient<Book>(httpClientFactory).Query;

      sut
        .Select(x => new
        {
          x.Title
        })
        .Where(x => x.Title == "Dracula" && x.Deleted)
        .Include(x => x.Author!)
        .PageSize(10)
        .PageNumber(2)
        .OrderBy(x => x.Title);

      var response = await sut.ToListAsync();
      response.Should().BeEquivalentTo([new Book() { Title = "Dracula", Id = 1 }]);
    }
    
    [Fact]
    public async Task Test5()
    {
      const string responseData = """
                                    {
                                      "links": {
                                        "self": "https://example.jsonapi.com/books",
                                        "next": "http://https://example.jsonapi.com/books?page[offset]=2",
                                        "last": "http://https://example.jsonapi.com/books?page[offset]=10"
                                      },
                                      "data": {
                                        "type": "book",
                                        "id": "1",
                                        "attributes": {
                                          "title": "Dracula"
                                        },
                                        "links": {
                                          "self": "https://example.jsonapi.com/books/1"
                                        }
                                      }
                                    }
                                  """;
      
      var httpClientFactory = Substitute.For<IHttpClientFactory>();
      httpClientFactory.CreateClient("api.books").Returns(GetMockedHttpClient("https://example.jsonapi.com", "/books/book/1?include=author&fields[book]=title,author&fields[author]=lastName", responseData));
      var sut = new JsonApiClient<Book>(httpClientFactory).Query;

      sut
        .Select(x => new
        {
          x.Title,
          x.Author
        })
        .Select<Author>(x => new { x.LastName })
        .Include(x => x.Author!);

      var response = await sut.FindAsync(1);
      response.Should().BeEquivalentTo(new Book() { Title = "Dracula", Id = 1 });
    }
    
    [Fact]
    public async Task Test6()
    {
      const string responseData = """
                                    {
                                      "links": {
                                        "self": "https://example.jsonapi.com/books",
                                        "next": "http://https://example.jsonapi.com/books?page[offset]=2",
                                        "last": "http://https://example.jsonapi.com/books?page[offset]=10"
                                      },
                                      "data": [{
                                        "type": "book",
                                        "id": "1",
                                        "attributes": {
                                          "title": "Dracula"
                                        },
                                        "links": {
                                          "self": "https://example.jsonapi.com/books/1"
                                        }
                                      }]
                                    }
                                  """;
      
      var httpClientFactory = Substitute.For<IHttpClientFactory>();
      httpClientFactory.CreateClient("api.books").Returns(GetMockedHttpClient("https://example.jsonapi.com", "/books/book?include=author&include=tagsBooks&fields[book]=title,author&fields[author]=lastName&page[size]=tagsBooks:20&page[number]=tagsBooks:1", responseData));
      var sut = new JsonApiClient<Book>(httpClientFactory).Query;

      sut
        .Select(x => new
        {
          x.Title,
          x.Author
        })
        .Select<Author>(x => new { x.LastName })
        .Include(x => x.Author!)
        .Include(x => x.TagsBooks)
        .PageSize(20, x => x.TagsBooks)
        .PageNumber(1, x => x.TagsBooks);

      var response = await sut.ToListAsync();
      response.First().Should().BeEquivalentTo(new Book() { Title = "Dracula", Id = 1 });
    }

    private static HttpClient GetMockedHttpClient(string baseUrl, string url, string response)
    {
        var messageHandlerMock = new MockHttpMessageHandler();
        messageHandlerMock.When(url)
          .Respond("application/vnd.api+json", response);
        HttpClient client = new(messageHandlerMock);
        client.BaseAddress = new Uri(baseUrl);
        return client;
    }
}