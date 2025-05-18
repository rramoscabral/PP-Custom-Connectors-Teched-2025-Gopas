using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDemo.DataLayer.Models;

/// <summary>
/// GitHubUser entity representing a user in the GitHub system.
/// </summary>
[Table("GitHubUsers", Schema = "CustomConnector")]
public class GitHubUser
{
    /// <summary>
    /// Identifier for the GitHub user record in the database.
    /// </summary>
    [Key]
    public int GitHubUserId { get; set; }

    /// <summary>
    /// GitHub username.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Login { get; set; }

    /// <summary>
    /// GitHub user 
    /// </summary>
    [StringLength(200)]
    public string? AvatarUrl { get; set; }

    [StringLength(200)]
    public string? ProfileUrl { get; set; }

    public ICollection<GitHubIssue> Issues { get; set; }
}
