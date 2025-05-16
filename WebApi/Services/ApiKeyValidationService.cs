using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity

namespace MyAppDemo.WebAPI.Services;

/// <summary>
/// Service for validating API keys.
/// </summary>
public class ApiKeyValidationService : IApiKeyValidationService
{
    private readonly WebAPIDbContext _dbContext;


    public ApiKeyValidationService(WebAPIDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuthorizedEmail?> ValidateApiKeyAsync(string apiKey)
    {
        return await _dbContext.AuthorizedEmails
            .FirstOrDefaultAsync(u => u.ApiKey == apiKey);
    }
}
