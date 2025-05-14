using MyAppDemo.DataLayer.Models; // To access the entity

namespace MyAppDemo.WebAPI.Services;

public interface IGitHubService
{
    Task AddRepositoryAsync(string ownerName, string repositoryName, string email);
    Task RemoveRepositoryAsync(string ownerName, string repositoryName, string email);
}
