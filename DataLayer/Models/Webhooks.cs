using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAppDemo.DataLayer.Models;


[Table("Webhooks", Schema = "CustomConnector")]
public class Webhook
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(500)]
    public required string WebhookUrl { get; set; }
    
    [Required]
    [EnumDataType(typeof(WebhookType))]
    public WebhookType Type { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [StringLength(100)]
    public string? FlowId { get; set; }

    public DateTime? LastTrigger { get; set; }
}