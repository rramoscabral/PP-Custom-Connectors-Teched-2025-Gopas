using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyAppDemo.DataLayer.Models;
using System.IO;

namespace MyAppDemo.DataLayer.DBContext;

public class WebAPIDbContext : DbContext
{
    public WebAPIDbContext(DbContextOptions<WebAPIDbContext> options) 
        : base(options)
    {

    }

    public DbSet<Webhook> Webhooks { get; set; }
    public DbSet<GitHubRepository> GitHubRepositories { get; set; }
    public DbSet<GitHubUser> GitHubUsers { get; set; }
    public DbSet<GitHubIssue> GitHubIssues { get; set; }
    public DbSet<AuthorizedEmail> AuthorizedEmails { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Webhook>()
            .HasIndex(w => w.WebhookUrl)
            .IsUnique();
            
        modelBuilder.Entity<GitHubRepository>()
            .HasIndex(r => new { r.OwnerName, r.RepositoryName, r.WebhookSecret, r.Email })
            .IsUnique();
            
        modelBuilder.Entity<AuthorizedEmail>()
            .HasIndex(a => new { a.Email, a.Service })
            .IsUnique();

        modelBuilder.Entity<GitHubRepository>()
         .HasMany(r => r.Issues)
         .WithOne(i => i.Repository)
         .HasForeignKey(i => i.RepositoryId)
         .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GitHubUser>()
         .HasMany(u => u.Issues)
         .WithOne(i => i.User)
         .HasForeignKey(i => i.UserId)
         .OnDelete(DeleteBehavior.Cascade);


    }
}