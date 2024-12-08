using JsonApiClient.Attributes;
using JsonApiClient.Models;

namespace JsonApiClient.Tests.Models;

[JRes("api.books", "tags-books")]
public sealed class BooksTags : JResource<int>
{
    [JAttr]
    public int BookId { get; set; }
    [JAttr]
    public int TagId { get; set; }
    [JRel]
    public Book Book { get; set; }
    [JRel]
    public Tag Tag { get; set; }
}