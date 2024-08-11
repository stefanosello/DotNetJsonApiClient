using JsonApiClient.Attributes;
using JsonApiDotNetCore.Resources;

namespace JsonApiClient.Tests.Models;

[JsonApiEntity("books")]
public class Author : Identifiable<int>
{
    public override int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public DateTime DateOfBirth { get; set; }

    public virtual ICollection<Book> Books { get; set; } = [];
}