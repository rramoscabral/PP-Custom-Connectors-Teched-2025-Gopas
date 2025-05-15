namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for GitHub repository request.
/// </summary>
public class GitHubRepositoryRequest
{
    public required string OwnerName { get; set; }

    public required string RepositoryName { get; set; }

    // The WebhookSecret is automatically generated on the server, so it should not be part of the input DTO. 
    //public required string WebhookSecret { get; set; }
    
    public required string Email { get; set; }
}
