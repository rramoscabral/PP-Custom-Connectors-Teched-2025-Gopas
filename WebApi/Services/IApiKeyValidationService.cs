using MyAppDemo.DataLayer.Models; // To access the entity


namespace MyAppDemo.WebAPI.Services;

/// <summary>
/// Service for validating API keys.
/// </summary>
public interface IApiKeyValidationService
{
    Task<AuthorizedEmail?> ValidateApiKeyAsync(string apiKey);
}
