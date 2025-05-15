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
    [Required]
    public int Id { get; set; }

    [Required]
    public int Number { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    public string Body { get; set; }

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

    [Required]
    public string WebhookSecret { get; set; }
}

public class GitHubUserDto
{
    [Required]
    [StringLength(100)]
    public string Login { get; set; }

    [StringLength(200)]
    public string Avatar_Url { get; set; }

    [StringLength(200)]
    public string Html_Url { get; set; }
}
