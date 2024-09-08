using JsonApiClient.Attributes;

namespace JsonApiClient.Tests.Models;

[JsonApiEntity("books")]
public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? PublishDate { get; set; } = null;
    
    public bool Annullato { get; set; }

    public virtual Author? Author { get; set; } = null;
}