using JsonApiClient.Attributes;

namespace JsonApiClient.Tests.Models;

[JsonApiEntity("books")]
public class Author
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public DateTime DateOfBirth { get; set; }

    public virtual ICollection<Book> Books { get; set; } = [];
}