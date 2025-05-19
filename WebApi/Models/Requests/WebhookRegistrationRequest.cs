using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for webhook registration request.
/// </summary>
public class WebhookRegistrationRequest
{
    /// <summary>
    /// Email address of the user registering the Webhook.
    /// </summary>
    [Required]
    [Display(Name = "Email", Description = "Email address of the user registering the Webhook.")]
    public required string Email { get; set; }

    /// <summary>
    /// Webhook Callback URL.
    /// </summary>
    [Required]
    [Display(Name = "WebhookUrl", Description = "Webhook Callback URL.")]
    public required string WebhookUrl { get; set; }

    /// <summary>
    /// Flow identification name of your choice.
    /// </summary>
    [Required]
    [Display(Name = "Flow Identifier", Description = "Unique Flow Identifier in Power Automate.")]
    public required string FlowId { get; set; }

    /// <summary>
    /// GitHub repository owner username.
    /// </summary>
    [Display(Name = "GitHub repository owner username", Description = "GitHub repository owner username.")]
    public string? OwnerUsername { get; set; }

    /// <summary>
    /// GitHub repository name as it appears in the URL.
    /// </summary>
    [Display(Name = "GitHub repository name", Description = "GitHub repository name as it appears in the URL.")]
    public string? RepositoryName { get; set; }

}
