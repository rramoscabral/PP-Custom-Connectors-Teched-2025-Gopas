namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for webhook registration request.
/// </summary>
public class WebhookRegistrationRequest
{
    public required string Email { get; set; }
    public required string WebhookUrl { get; set; }
    public string? FlowId { get; set; }
}
