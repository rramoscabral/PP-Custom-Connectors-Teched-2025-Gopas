namespace MyAppDemo.WebAPI.Models.Requests;


/// <summary>
/// DTO for GitHub issue creation request.
/// </summary>
public class GitHubIssueRequest
{
    public string RepositoryOwner { get; set; }
    public string RepositoryName { get; set; }
    public object Issue { get; set; }
}
