using MyAppDemo.DataLayer.Models; // To access the entity

namespace MyAppDemo.WebAPI.Services;

/// <summary>
/// Service for managing GitHub repositories.
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Adds a GitHub repository to the user's account.
    /// </summary>
    /// <param name="ownerName"></param>
    /// <param name="repositoryName"></param>
    /// <param name="WebhookSecret"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    Task AddRepositoryAsync(string ownerName, string repositoryName, string WebhookSecret, string email);

    /// <summary>
    /// Removes a GitHub repository from the user's account.
    /// </summary>
    /// <param name="ownerName"></param>
    /// <param name="repositoryName"></param>
    /// <param name="WebhookSecret"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    Task RemoveRepositoryAsync(string ownerName, string repositoryName, string WebhookSecret, string email);
}
