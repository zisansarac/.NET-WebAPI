using first_.NET_project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace first_.NET_project.Data;


public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //identity sistemini bozmadan yapacağız.
        base.OnModelCreating(builder);

        // böylelikle url üretirken benzersiz üreteceğiz. indexlemek: hızlı sorgular yapmak
        builder.Entity<BlogPost>()
             .HasIndex(b => b.Slug)
             .IsUnique();

        builder.Entity<BlogPost>()
             .HasIndex(b => b.AuthorId);
    }
    
    
}

