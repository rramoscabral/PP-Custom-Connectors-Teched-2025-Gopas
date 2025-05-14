namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for Perplexity request.
/// </summary>
public class PerplexityRequest
{
    public string Prompt { get; set; }
    public string Email { get; set; }
}
