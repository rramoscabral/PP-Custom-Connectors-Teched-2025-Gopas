using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDemo.DataLayer.Models;

[Table("GitHubIssues", Schema = "CustomConnector")]
public class GitHubIssue
{

    [Key]
    public int Id { get; set; }

    [Required]
    public int IssueNumber { get; set; }

    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    public string? Body { get; set; }

    [Required]
    [StringLength(100)]
    public string UserLogin { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Url]
    public string Html_Url { get; set; }

    // Foreign Key to Repository
    [Required]
    public int RepositoryId { get; set; }

    [ForeignKey("RepositoryId")]
    public GitHubRepository Repository { get; set; }

    // Foreign Key to GitHubUser
    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public GitHubUser User { get; set; }
}