using MyAppDemo.DataLayer.Models; // To access the entity


namespace MyAppDemo.WebAPI.Services;


public interface IApiKeyValidationService
{
    Task<AuthorizedEmail?> ValidateApiKeyAsync(string apiKey);
}
