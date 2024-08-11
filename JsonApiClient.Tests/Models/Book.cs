using JsonApiClient.Attributes;
using JsonApiDotNetCore.Resources;

namespace JsonApiClient.Tests.Models;

[JsonApiEntity("books")]
public class Book : Identifiable<int>
{
    public override int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? PublishDate { get; set; } = null;

    public virtual Author? Author { get; set; } = null;
}