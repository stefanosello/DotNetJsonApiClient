using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JsonApiClient.Example.Server.Models;

[Resource]
[Table("Books")]
[PrimaryKey(nameof(Id))]
public class Book : Identifiable<int>
{
    [Column("BookID")]
    public override int Id { get; set; }
    
    [Attr]
    [Column("AuthorID")]
    public int AuthorId { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(255)")]
    public string? Title { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(20)")]
    public string? Isbn { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(MAX)")]
    public string? Summary { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(MAX)")]
    public string? Cover { get; set; }
    
    [Attr]
    public DateOnly PublishedDate { get; set; }
    
    [Attr]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    [Attr]
    public int StockQuantity { get; set; }

    [HasOne]
    public required Author Author { get; set; }
    
    [HasMany]
    public virtual ICollection<Tag> Tags { get; set; } = [];
    
    public virtual ICollection<BookTag> BooksTags { get; set; } = [];

}