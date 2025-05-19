using System.ComponentModel.DataAnnotations;

namespace MyAppDemo.WebAPI.Models.Requests;

/// <summary>
/// DTO for Perplexity request.
/// </summary>
public class PerplexityRequest
{

    /// <summary>
    /// User prompt to generate AI content.
    /// </summary>
    [Required]
    [Display(Name = "Prompt", Description = "User prompt to generate AI content.")]
    public required string Prompt { get; set; }


    /// <summary>
    /// User e-mail.
    /// </summary>
    [Required]
    [Display(Name = "Email", Description = "User email address.")]
    public required string Email { get; set; }
}
