using JsonApiClient.Attributes;

namespace JsonApiClient.Tests.Models;

[JRes("books")]
public class Author
{
    public int Id { get; set; }
    [JAttr]
    public string FirstName { get; set; }
    [JAttr]
    public string LastName { get; set; }
    [JAttr]
    public DateTime DateOfBirth { get; set; }
    [JRel]
    public virtual ICollection<Book> Books { get; set; } = [];
}