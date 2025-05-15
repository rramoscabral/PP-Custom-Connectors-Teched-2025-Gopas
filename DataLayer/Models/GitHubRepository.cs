using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAppDemo.DataLayer.Models;

[Table("GitHubRepositories", Schema = "CustomConnector")]
public class GitHubRepository
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string OwnerName { get; set; }

    [Required]
    [StringLength(100)]
    public required string RepositoryName { get; set; }

    [Required]
    [StringLength(200)]
    public required string WebhookSecret { get; set; }

    [Required]
    [StringLength(100)]
    public required string Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<GitHubIssue> Issues { get; set; }
}