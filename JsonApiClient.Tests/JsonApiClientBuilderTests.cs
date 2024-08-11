using FluentAssertions;
using JsonApiClient.Tests.Models;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace JsonApiClient.Tests;

public class JsonApiClientBuilderTests
{
    [Fact]
    public async Task Test1()
    {
        var responseData = """
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
        var sut = new JsonApiClientBuilder<Book>("https://example.jsonapi.com");
        var messageHandlerMock = new MockHttpMessageHandler();
        messageHandlerMock.When("https://example.jsonapi.com/books/book?fields[book]=title,id")
            .Respond("application/vnd.api+json", responseData);
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient("jsonApiTests").Returns(new HttpClient(messageHandlerMock));

        sut.SetHttpClient(httpClientFactory, "jsonApiTests");
        sut.Select<Book>(x => new
        {
            x.Title,
            x.Id
        });

        var jsonApiClient = sut.Build();
        var response = await jsonApiClient.GetAsync();
        response.Should().BeEquivalentTo(new Book() { Title = "Dracula", Id = 1 });
    }
}