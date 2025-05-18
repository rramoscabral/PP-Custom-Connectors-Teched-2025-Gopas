using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyAppDemo.DataLayer.Models;

/// <summary>
/// AuthorizedEmail entity representing an authorized email in the system.
/// </summary>
[Table("AuthorizedEmails", Schema = "CustomConnector")]
public class AuthorizedEmail
{
    /// <summary>
    /// Identifier for the authorized email record in the database.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Email address of the user to be authorized.
    /// </summary>
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// Service type associated with the email.
    /// </summary>
    [Required]
    [EnumDataType(typeof(ServiceType))]
    public ServiceType Service { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// API key associated with the email.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ApiKey { get; set; } = string.Empty;

}