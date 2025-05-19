using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for GitHub repository request.
/// </summary>
public class GitHubRepositoryRequest
{
    /// <summary>
    /// The name of the owner of the GitHub repository.
    /// </summary>
    [Required]
    [Display(Name = "Repository owner username", Description = "The name of the owner of the GitHub repository.")]
    public required string OwnerName { get; set; }

    /// <summary>
    /// The name of the GitHub repository.
    /// </summary>
    [Required]
    [Display(Name = "Repository name", Description = "The name of the GitHub repository as shown in the URL.")]
    public required string RepositoryName { get; set; }

    // The WebhookSecret is automatically generated on the server, so it should not be part of the input DTO. 
    //public required string WebhookSecret { get; set; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    [Required]
    [Display(Name = "Email", Description = "The email address of the user.")]
    public required string Email { get; set; }
}
