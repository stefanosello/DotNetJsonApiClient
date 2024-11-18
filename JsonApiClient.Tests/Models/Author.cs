using JsonApiClient.Attributes;
using JsonApiClient.Interfaces;
using JsonApiClient.Models;

namespace JsonApiClient.Tests.Models;

[JRes("api.books", "books", "author")]
public class Author : JResource<int>
{
    [JAttr]
    public string? FirstName { get; set; }
    [JAttr]
    public string? LastName { get; set; }
    [JAttr]
    public DateTime DateOfBirth { get; set; }
    [JRel]
    public virtual ICollection<Book> Books { get; set; } = [];
}