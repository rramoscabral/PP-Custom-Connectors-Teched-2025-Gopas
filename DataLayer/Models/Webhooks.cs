using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAppDemo.DataLayer.Models;

/// <summary>
/// Webhook entity representing a webhook configuration in the system.
/// </summary>
[Table("Webhooks", Schema = "CustomConnector")]
public class Webhook
{

    /// <summary>
    /// Identifier for the webhook record in the database.
    /// </summary>
    [Key]
    public int WebhookId { get; set; }

    /// <summary>
    /// Email address of the user who created the webhook.
    /// </summary>
    [Required]
    [StringLength(100)]
    public required string Email { get; set; }


    /// <summary>
    /// Callback URL for the webhook.
    /// </summary>
    [Required]
    [StringLength(500)]
    public required string WebhookUrl { get; set; }

    /// <summary>
    /// Webhook type.
    /// </summary>
    [Required]
    [EnumDataType(typeof(WebhookType))]
    public WebhookType Type { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Foreign Key for Webhook
    /// </summary>
    [StringLength(100)]
    public string? FlowId { get; set; }

    /// <summary>
    /// Last time the webhook was triggered.
    /// </summary>
    public DateTime? LastTrigger { get; set; }


    /// <summary>
    /// Reverse navigation for GitHubRepository.
    /// </summary>
    public ICollection<GitHubRepository> Repositories { get; set; }

}