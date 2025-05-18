using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDemo.DataLayer.Models;

/// <summary>
/// GitHubIssue entity representing a GitHub issue in the system.
/// </summary>
[Table("GitHubIssues", Schema = "CustomConnector")]
public class GitHubIssue
{
    /// <summary>
    /// GitHub issue identifier record in the database.
    /// </summary>
    [Key]
    public int GitHubIssueId { get; set; }


    /// <summary>
    /// Github issue number.
    /// </summary>
    [Required]
    public int IssueNumber { get; set; }

    /// <summary>
    /// Github issue title.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// GitHub issue body.
    /// </summary>
    public string? Body { get; set; }


    /// <summary>
    /// GitHub username.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string UserLogin { get; set; }

    /// <summary>
    /// Datetime when this issue was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// GitHub issue url.
    /// </summary>
    [Required]
    [Url]
    public string Html_Url { get; set; }

    /// <summary>
    /// Repository Identifier for the issue.
    /// </summary>
    [Required]
    public int RepositoryId { get; set; }

    /// <summary>
    /// Foreign Key to Repository
    /// </summary>
    [ForeignKey("RepositoryId")]
    public GitHubRepository Repository { get; set; }

    /// <summary>
    /// GitHub user identifier for the issue.
    /// </summary>
    [Required]
    public int UserId { get; set; }


    /// <summary>
    /// Foreign Key to GitHubUser.
    /// </summary>
    [ForeignKey("UserId")]
    public GitHubUser User { get; set; }
}