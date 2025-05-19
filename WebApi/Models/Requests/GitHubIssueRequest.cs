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
    [Display(Name = "", Description = ".")]
    public string Action { get; set; }

    /// <summary>
    /// The issue object that contains information about the issue.
    /// </summary>
    [Required]
    [Display(Name = "", Description = ".")]
    public GitHubIssueDto Issue { get; set; }

    /// <summary>
    /// The repository object that contains information about the repository.
    /// </summary>
    [Required]
    [Display(Name = "", Description = ".")]
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
    [Display(Name = "", Description = ".")]
    public int Number { get; set; }

    /// <summary>
    /// The title of the issue.
    /// </summary>
    [StringLength(200)]
    [Display(Name = "", Description = ".")]
    public string? Title { get; set; }

    /// <summary>
    /// The body of the issue.
    /// </summary>
    [Display(Name = "", Description = ".")]
    public string? Body { get; set; }

    /// <summary>
    /// The user who created the issue.
    /// </summary>
    [Required]
    [Display(Name = "", Description = ".")]
    public required GitHubUserDto User { get; set; }

    /// <summary>
    /// The url of the issue on GitHub.
    /// </summary>
    [Required]
    [Url]
    [Display(Name = "", Description = ".")]
    public string? Html_Url { get; set; }

    /// <summary>
    /// Date and time record in database.
    /// </summary>
    [Required]
    [Display(Name = "", Description = ".")]
    public DateTime Created_At { get; set; }
}

public class GitHubRepositoryDto
{
    /// <summary>
    /// GitHub repository name.
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "", Description = ".")]
    public required string Name { get; set; }

    /// <summary>
    /// GitHub owner of the repository.
    /// </summary>
    [Required]
    [Display(Name = "", Description = ".")]
    public required GitHubUserDto Owner { get; set; }

    /**
     * GitHub does not send the secret in the JSON body. Instead, it uses the secret to generate an HMAC signature that is sent in an HTTP header called:
     * X-Hub-Signature(for SHA1)
     * X-Hub-Signature-256 (for SHA256)
     */
    //[Required]
    //public string WebhookSecret { get; set; }
}

public class GitHubUserDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "", Description = ".")]
    public required string Login { get; set; }

    /// <summary>
    /// Avatar URL of the user.
    /// </summary>
    [StringLength(200)]
    [Display(Name = "", Description = ".")]
    public string? Avatar_Url { get; set; }

    /// <summary>
    /// The URL of the user's profile on GitHub.
    /// </summary>
    [StringLength(200)]
    [Display(Name = "", Description = ".")]
    public string? Html_Url { get; set; }
}
