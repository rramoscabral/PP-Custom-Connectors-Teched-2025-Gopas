namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for webhook registration request.
/// </summary>
public class WebhookRegistrationRequest
{
    public string Email { get; set; }
    public string WebhookUrl { get; set; }
    public string FlowId { get; set; }
}
