using System.ComponentModel.DataAnnotations;



/// <summary>
/// DTO for GitHub issue creation request.
/// </summary>
public class GitHubIssueRequest
{
    public required string RepositoryOwner { get; set; }
    public required string RepositoryName { get; set; }
    public required object Issue { get; set; }
}
