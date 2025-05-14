namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for GitHub repository request.
/// </summary>
public class GitHubRepositoryRequest
{
    public required string OwnerName { get; set; }
    public required string RepositoryName { get; set; }
    public required string Email { get; set; }
}
