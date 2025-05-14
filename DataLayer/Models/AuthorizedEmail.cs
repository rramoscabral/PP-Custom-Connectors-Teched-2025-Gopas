using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyAppDemo.DataLayer.Models;


 [Table("AuthorizedEmails", Schema = "CustomConnector")]
public class AuthorizedEmail
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [EnumDataType(typeof(ServiceType))]
    public ServiceType Service { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [Required]
    [StringLength(100)]
    public string ApiKey { get; set; } = string.Empty;

}