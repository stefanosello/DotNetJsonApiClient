using JsonApiClient.Attributes;

namespace JsonApiClient.Tests.Models;

[JRes("books")]
public class Book
{
    public int Id { get; set; }
    [JAttr]
    public string Title { get; set; } = null!;
    [JAttr]
    public DateTime? PublishDate { get; set; } = null;
    [JAttr]
    public bool Annullato { get; set; }
    [JRel]
    public virtual Author? Author { get; set; } = null;
}