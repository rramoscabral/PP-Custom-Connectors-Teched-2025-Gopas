using MyAppDemo.WebAPI.Models.Requests;
using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Response;

/// <summary>
/// DTO for GitHub issue response payload.
/// </summary>
public class GitHubIssueResponse
{

    /// <summary>
    /// The title of the issue.
    /// </summary>
    [StringLength(200)]
    [Display(Name = "Title", Description = "The title of the issue.")]
    public string? Title { get; set; }

    /// <summary>
    /// The body of the issue.
    /// </summary>
    [Display(Name = "Body", Description = "The body of the issue.")]
    public string? Body { get; set; }

    /// <summary>
    /// The url of the issue on GitHub.
    /// </summary>
    [Required]
    [Url]
    [Display(Name = "Issue URL", Description = "The url of the issue on GitHub.")]
    public string? Html_Url { get; set; }


    /// <summary>
    /// The user who created the issue.
    /// </summary>
    [Required]
    [Display(Name = "The user who created the issue", Description = "The user who created the issue.")]
    public required string User { get; set; }

    /// <summary>
    /// GitHub repository name as it appears in the URL..
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "GitHub Repository name", Description = "GitHub repository name as it appears in the URL.")]
    public required string RepositoryName { get; set; }

    /// <summary>
    /// GitHub owner of the repository.
    /// </summary>
    [Required]
    [Display(Name = "GitHub repository owner", Description = "GitHub owner of the repository.")]
    public required string RepositoryOwner { get; set; }

}
