using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for GitHub issue webhook payload.
/// </summary>
public class GitHubIssueRequest
{
    /// <summary>
    /// The action that triggered the webhook. Possible values are: opened, edited, deleted, assigned, unassigned, labeled, unlabeled, locked, unlocked.
    /// </summary>
    [Required]
    [Display(Name = "Action", Description = "The action that triggered the webhook. Possible values are: opened, edited, deleted, assigned, unassigned, labeled, unlabeled, locked, unlocked.")]
    public string Action { get; set; }

    /// <summary>
    /// The issue object that contains information about the issue.
    /// </summary>
    [Required]
    [Display(Name = "Issue", Description = "The issue object that contains information about the issue.")]
    public GitHubIssueDto Issue { get; set; }

    /// <summary>
    /// The repository object that contains information about the repository.
    /// </summary>
    [Required]
    [Display(Name = "Repository", Description = "The repository object that contains information about the repository.")]
    public GitHubRepositoryDto Repository { get; set; }
}

public class GitHubIssueDto
{
    // GitHubIssueId is from the database and not from GitHub. So, it is not required.
    //public int GitHubIssueId { get; set; }

    /// <summary>
    /// The unique identifier of the issue.
    /// </summary>
    [Required]
    [Display(Name = "Number", Description = "The unique identifier of the GitHub issue.")]
    public int Number { get; set; }

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
    /// The user who created the issue.
    /// </summary>
    [Required]
    [Display(Name = "The user who created the issue", Description = "The user who created the issue.")]
    public required GitHubUserDto User { get; set; }

    /// <summary>
    /// The url of the issue on GitHub.
    /// </summary>
    [Required]
    [Url]
    [Display(Name = "Issue URL", Description = "The url of the issue on GitHub.")]
    public string? Html_Url { get; set; }

    /// <summary>
    /// Date and time record in database.
    /// </summary>
    [Required]
    [Display(Name = "Created at", Description = "Date and time that was record in WebAPI.")]
    public DateTime Created_At { get; set; }
}

public class GitHubRepositoryDto
{
    /// <summary>
    /// GitHub repository name as it appears in the URL.
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "GitHub Repository name", Description = "GitHub repository name as it appears in the URL.")]
    public required string Name { get; set; }

    /// <summary>
    /// GitHub owner of the repository.
    /// </summary>
    [Required]
    [Display(Name = "GitHub repository owner", Description = "GitHub owner of the repository.")]
    public required GitHubUserDto Owner { get; set; }


    // GitHub does not send the secret in the JSON body. Instead, it uses the secret to generate an HMAC signature that is sent in an HTTP header called:
    // X-Hub-Signature(for SHA1)
    // X-Hub-Signature-256 (for SHA256)

    //[Required]
    //public string WebhookSecret { get; set; }
}

public class GitHubUserDto
{
    /// <summary>
    /// The unique identifier of the user on GitHub.
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "GitHub username", Description = "The unique identifier of the user on GitHub.")]
    public required string Login { get; set; }

    /// <summary>
    /// The URL of the user avatar URL on GitHub.
    /// </summary>
    [StringLength(200)]
    [Display(Name = "GitHub user avatar URL", Description = "The URL of the user avatar URL on GitHub.")]
    public string? Avatar_Url { get; set; }

    /// <summary>
    /// The URL of the user profile on GitHub.
    /// </summary>
    [StringLength(200)]
    [Display(Name = "GitHub user profile URL", Description = "The URL of the user profile on GitHub.")]
    public string? Html_Url { get; set; }
}
