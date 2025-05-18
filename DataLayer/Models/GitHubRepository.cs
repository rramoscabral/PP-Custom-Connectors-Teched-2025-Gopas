using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAppDemo.DataLayer.Models;


/// <summary>
/// GitubRepository entity representing a GitHub repository in the system.
/// </summary>
[Table("GitHubRepositories", Schema = "CustomConnector")]
public class GitHubRepository
{
    /// <summary>
    /// GitHub repository identifier record in the database.
    /// </summary>
    [Key]
    public int GitHubRepoId { get; set; }

    /// <summary>
    /// GitHub repository owner name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string OwnerName { get; set; }


    /// <summary>
    /// GiHub repository name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string RepositoryName { get; set; }

    /// <summary>
    /// Secret for Github repository webhook.
    /// </summary>
    [Required]
    [StringLength(200)]
    public required string WebhookSecret { get; set; }

    /// <summary>
    /// E-mail address of the user who created the repository.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string Email { get; set; }

    /// <summary>
    /// Datetime when this record was created in the database.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Github issues associated with this repository.
    /// </summary>
    public ICollection<GitHubIssue> Issues { get; set; }


    /// <summary>
    /// Foreign key Webhook Identifier record in the database for the repository.
    /// </summary>
    [ForeignKey("Webhook")]
    public int? WebhookId { get; set; }

    /// <summary>
    /// Navigation property to the Webhook entity.
    /// </summary>
    public Webhook Webhook { get; set; }

}