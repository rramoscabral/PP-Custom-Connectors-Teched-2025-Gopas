using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity


namespace MyAppDemo.WebAPI.Services;

/// <summary>
/// Service for managing GitHub repositories.
/// </summary>
public class GitHubService : IGitHubService
{
    private readonly WebAPIDbContext _context;

    public GitHubService(WebAPIDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a GitHub repository to the user's account.
    /// </summary>
    /// <param name="ownerName"></param>
    /// <param name="repositoryName"></param>
    /// <param name="webhookSecret"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task AddRepositoryAsync(string ownerName, string repositoryName, string webhookSecret, string email)
    {
        var repo = new GitHubRepository
        {
            OwnerName = ownerName,
            RepositoryName = repositoryName,
            WebhookSecret = webhookSecret,
            Email = email
        };
        _context.GitHubRepositories.Add(repo);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a GitHub repository from the user's account.
    /// </summary>
    /// <param name="ownerName"></param>
    /// <param name="repositoryName"></param>
    /// <param name="webhookSecret"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task RemoveRepositoryAsync(string ownerName, string repositoryName, string webhookSecret, string email)
    {
        var repo = await _context.GitHubRepositories
            .FirstOrDefaultAsync(r => r.OwnerName == ownerName && r.RepositoryName == repositoryName && r.WebhookSecret == webhookSecret && r.Email == email);
    
        if (repo != null)
        {
            _context.GitHubRepositories.Remove(repo);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Retrieves all GitHub repositories associated with a specific email address.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GitHubRepository>> GetRepositoriesByEmailAsync(string email)
    {
        return await _context.GitHubRepositories.Where(r => r.Email == email).ToListAsync();
    }
}
