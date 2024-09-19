using JsonApiClient.Attributes;

namespace JsonApiClient.Tests.Models;

[JsonApiResource("books")]
public class Book
{
    public int Id { get; set; }
    [JsonApiAttribute]
    public string Title { get; set; } = null!;
    [JsonApiAttribute]
    public DateTime? PublishDate { get; set; } = null;
    [JsonApiAttribute]
    public bool Annullato { get; set; }
    [JsonApiRelationship]
    public virtual Author? Author { get; set; } = null;
}