using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JsonApiClient.Example.Server.Models;

[Resource]
[Table("Tags")]
[PrimaryKey(nameof(Id))]
public class Tag : Identifiable<int>
{
    [Column("TagID")]
    public override int Id { get; set; }
    
    [Attr]
    [Column(TypeName = "nvarchar(255)")]
    public string? Label { get; set; }
    
    [Attr]
    [Column("ParentTagID")]
    public int? ParentId { get; set; }
    
    [HasOne]
    public virtual Tag? Parent { get; set; }
    
    [HasMany]
    public virtual ICollection<Tag> Children { get; set; } = [];
    
    [HasMany]
    public virtual ICollection<Book> Books { get; set; } = [];
    
    public virtual ICollection<BookTag> BooksTags { get; set; } = [];
}