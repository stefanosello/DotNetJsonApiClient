using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JsonApiClient.Example.Server.Models;

[Table("BooksTags")]
[NoResource]
[PrimaryKey(nameof(BookId), nameof(TagId))]
public class BookTag
{
    [Column("BookID", TypeName = "int")]
    public required int BookId { get; set; }
    [Column("TagID", TypeName = "int")]
    public required int TagId { get; set; }
    public required Book Book { get; set; }
    public required Tag Tag { get; set; }
}