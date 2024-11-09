using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JsonApiClient.Example.Server.Models;

[Resource]
[Table("Authors")]
[PrimaryKey(nameof(Id))]
public class Author : Identifiable<int>
{
    [Column("AuthorID")]
    public override int Id { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(100)")]
    public string? FirstName { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(100)")]
    public string? LastName { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(255)")]
    public string? Email { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(MAX)")]
    public string? Avatar { get; set; }
    
    [Attr]
    [Column(TypeName = "date")]
    public DateOnly DateOfBirth { get; set; }
    
    [Attr]
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
    
    [HasMany]
    public virtual ICollection<Book> Books { get; set; } = [];
}