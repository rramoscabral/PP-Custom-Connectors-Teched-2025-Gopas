namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for Perplexity request.
/// </summary>
public class PerplexityRequest
{
    public required string Prompt { get; set; }
    public required string Email { get; set; }
}
