using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyAppDemo.WebAPI.Models.Requests; // To access the DTO (request)
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity
using MyAppDemo.WebAPI.Services;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization; // To access the service

namespace MyAppDemo.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    
    private readonly IWebhookService _webhookService;
    private readonly WebAPIDbContext _context;
    
    public GitHubController(IWebhookService webhookService, WebAPIDbContext context)
    {
        _webhookService = webhookService;
        _context = context;
    }
    
    [HttpPost("register-repository")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterRepository([FromBody] GitHubRepositoryRequest request)
    {

        // Check if it already exists
        var exists = await _context.GitHubRepositories.AnyAsync(r =>
            r.OwnerName == request.OwnerName &&
            r.RepositoryName == request.RepositoryName);

        if (exists)
            return BadRequest("Repository already registered.");

        // Generates a secure secret
        var secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var repository = new GitHubRepository
        {
            OwnerName = request.OwnerName,
            RepositoryName = request.RepositoryName,
            WebhookSecret = secret,  
            Email = request.Email
        };
        
        _context.GitHubRepositories.Add(repository);
        await _context.SaveChangesAsync();
        

        // Get domain dynamically
        var domain = $"{Request.Scheme}://{Request.Host}";
        var webhookUrl = $"{domain}/api/github-issue";
        
        return Ok(new
        {
            Message = "Repository registered successfully",
            WebhookSecret = secret,
            WebhookUrl = webhookUrl
        });

    }



    /// <summary>
    /// Receives a webhook from GitHub with information about a created or updated issue.
    /// Checks if there are any repositories registered with the provided owner and name.
    /// For each repository found, looks for the associated Webhook (Power Automate) and sends the issue data.
    /// Returns the main issue data as a response for dynamic use in Power Automate.
    /// </summary>
    /// <param name="request">object containing the GitHub issue, repository and user data.</param>
    /// <returns>HTTP response 200 with the edit data or 404 if the repository is not found.</returns>
    [AllowAnonymous]
    [HttpPost("issue-webhook")]
    public async Task<IActionResult> IssueWebhook([FromBody] GitHubIssueRequest request)
    {
        var repositories = await _context.GitHubRepositories
            .Where(r => r.OwnerName == request.Repository.Owner.Login && r.RepositoryName == request.Repository.Name && r.WebhookSecret == request.Repository.WebhookSecret)
            .ToListAsync();

        if (!repositories.Any())
            return NotFound("Repository not found.");

        var payload = new
        {
            title = request.Issue.Title,
            body = request.Issue.Body,
            html_url = request.Issue.Html_Url,
            user = request.Issue.User.Login,
            repository = request.Repository.Name,
            owner = request.Repository.Owner.Login
        };

        foreach (var repo in repositories)
        {
            var webhook = await _context.Webhooks
                .FirstOrDefaultAsync(w => w.Email == repo.Email && w.Type == WebhookType.GitHub);

            if (webhook == null)
                continue;

            await _webhookService.TriggerWebhook(webhook.WebhookUrl, WebhookType.GitHub, payload);
        }

        // Optional: Return the payload for debugging or direct use in Power Automate
        return Ok(payload);
    }



    /// <summary>
    /// Endpoint to register a Custom Connectors for GitHub.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("webhook")]
    public async Task<IActionResult> RegisterWebhook([FromBody] WebhookRegistrationRequest request)
    {
        await _webhookService.RegisterWebhook(
            request.Email,
            request.WebhookUrl,
            WebhookType.GitHub,
            request.FlowId);
            
        return Ok(new { Message = "Custom Connectors Webhook registered successfully" });
    }
    
    /// <summary>
    /// Endpoint to remove a Custom Connectors for GitHub.
    /// </summary>
    /// <param name="flowId"></param>
    /// <returns></returns>
    [HttpDelete("webhook/{flowId}")]
    public async Task<IActionResult> RemoveWebhook(string flowId)
    {
        var result = await _webhookService.RemoveWebhook(flowId);
        
        if (!result)
            return NotFound();
            
        return Ok(new { Message = "Custom Connectors Webhook removed successfully" });
    }
}
