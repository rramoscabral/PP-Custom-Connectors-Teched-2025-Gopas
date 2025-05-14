using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity


namespace MyAppDemo.WebAPI.Services;

public class GitHubService : IGitHubService
{
    private readonly WebAPIDbContext _context;

    public GitHubService(WebAPIDbContext context)
    {
        _context = context;
    }

    public async Task AddRepositoryAsync(string ownerName, string repositoryName, string email)
    {
        var repo = new GitHubRepository
        {
            OwnerName = ownerName,
            RepositoryName = repositoryName,
            Email = email
        };
        _context.GitHubRepositories.Add(repo);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRepositoryAsync(string ownerName, string repositoryName, string email)
    {
        var repo = await _context.GitHubRepositories
            .FirstOrDefaultAsync(r => r.OwnerName == ownerName && r.RepositoryName == repositoryName && r.Email == email);

        if (repo != null)
        {
            _context.GitHubRepositories.Remove(repo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<GitHubRepository>> GetRepositoriesByEmailAsync(string email)
    {
        return await _context.GitHubRepositories.Where(r => r.Email == email).ToListAsync();
    }
}
