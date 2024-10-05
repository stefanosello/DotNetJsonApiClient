using JsonApiClient.Attributes;
using JsonApiClient.Models;

namespace JsonApiClient.Tests.Models;

[JRes("api.books", "tags")]
public sealed class Tag : JResource<int>
{
    [JAttr]
    public string Label { get; set; }
    [JRel]
    public ICollection<TagsBooks> TagsBooks { get; set; }
}