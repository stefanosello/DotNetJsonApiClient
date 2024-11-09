using JsonApiClient.Example.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonApiClient.Example.Server;

public class ExampleDbContext : DbContext
{
    public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<BookTag> BooksTags { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Books)
            .UsingEntity<BookTag>(
                l => l.HasOne(bt => bt.Tag)
                    .WithMany(t => t.BooksTags)
                    .HasForeignKey(t => t.TagId)
                    .HasPrincipalKey(t => t.Id),
                r => r.HasOne(bt => bt.Book)
                    .WithMany(b => b.BooksTags)
                    .HasForeignKey(bt => bt.BookId)
                    .HasPrincipalKey(b => b.Id),
                j => j.HasKey(t => new { t.BookId, t.TagId }));
    }
}