using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for GitHub issue webhook payload.
/// </summary>
public class GitHubIssueRequest
{
    [Required]
    public string Action { get; set; }

    [Required]
    public GitHubIssueDto Issue { get; set; }

    [Required]
    public GitHubRepositoryDto Repository { get; set; }
}

public class GitHubIssueDto
{
    // Id is from the database and not from GitHub. So, it is not required.
    //public int Id { get; set; }

    [Required]
    public int Number { get; set; }

    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    public string? Body { get; set; }

    [Required]
    public GitHubUserDto User { get; set; }

    [Required]
    [Url]
    public string Html_Url { get; set; }

    [Required]
    public DateTime Created_At { get; set; }
}

public class GitHubRepositoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public GitHubUserDto Owner { get; set; }

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
    [Required]
    [StringLength(100)]
    public string Login { get; set; }

    [StringLength(200)]
    public string? Avatar_Url { get; set; }

    [StringLength(200)]
    public string? Html_Url { get; set; }
}
