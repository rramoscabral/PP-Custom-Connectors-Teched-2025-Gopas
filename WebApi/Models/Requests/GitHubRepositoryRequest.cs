namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for GitHub repository request.
/// </summary>
public class GitHubRepositoryRequest
{
    public string OwnerName { get; set; }
    public string RepositoryName { get; set; }
    public string Email { get; set; }
}
