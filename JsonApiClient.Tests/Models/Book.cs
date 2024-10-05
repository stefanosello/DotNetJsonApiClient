using JsonApiClient.Attributes;
using JsonApiClient.Models;

namespace JsonApiClient.Tests.Models;

[JRes("api.books", "books")]
public sealed class Book : JResource<int>
{
    [JAttr]
    public string? Title { get; set; }
    [JAttr]
    public DateTime? PublishDate { get; set; }
    [JAttr]
    public bool Deleted { get; set; }
    [JRel]
    public Author? Author { get; set; }
    [JRel]
    public ICollection<TagsBooks> TagsBooks { get; set; } = [];
}