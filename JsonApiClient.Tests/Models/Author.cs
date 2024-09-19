using JsonApiClient.Attributes;

namespace JsonApiClient.Tests.Models;

[JsonApiResource("books")]
public class Author
{
    public int Id { get; set; }
    [JsonApiAttribute]
    public string FirstName { get; set; }
    [JsonApiAttribute]
    public string LastName { get; set; }
    [JsonApiAttribute]
    public DateTime DateOfBirth { get; set; }
    [JsonApiRelationship]
    public virtual ICollection<Book> Books { get; set; } = [];
}