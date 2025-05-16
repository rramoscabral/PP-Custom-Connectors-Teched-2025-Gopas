namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for webhook registration request.
/// </summary>
public class WebhookRegistrationRequest
{
    /// <summary>
    /// Email address of the user registering the Webhook.
    public required string Email { get; set; }

    /// <summary>
    /// Webhook Callback URL.
    /// </summary>
    public required string WebhookUrl { get; set; }

    /// <summary>
    /// Flow identification name of your choice.
    /// </summary>
    public string? FlowId { get; set; }
}
