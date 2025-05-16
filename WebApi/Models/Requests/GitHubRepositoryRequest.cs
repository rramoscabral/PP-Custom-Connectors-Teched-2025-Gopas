namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for GitHub repository request.
/// </summary>
public class GitHubRepositoryRequest
{
    /// <summary>
    /// The name of the owner of the GitHub repository.
    /// </summary>
    public required string OwnerName { get; set; }

    /// <summary>
    /// The name of the GitHub repository.
    /// </summary>
    public required string RepositoryName { get; set; }

    // The WebhookSecret is automatically generated on the server, so it should not be part of the input DTO. 
    //public required string WebhookSecret { get; set; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public required string Email { get; set; }
}
