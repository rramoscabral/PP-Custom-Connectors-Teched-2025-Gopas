using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyAppDemo.WebAPI.Models.Requests; // To access the DTO (request)
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity
using MyAppDemo.WebAPI.Services; // To access the service

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
        var repository = new GitHubRepository
        {
            OwnerName = request.OwnerName,
            RepositoryName = request.RepositoryName,
            Email = request.Email
        };
        
        _context.GitHubRepositories.Add(repository);
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "Repository registered successfully" });
    }
    
   
    
    [HttpPost("issue-webhook")]
    public async Task<IActionResult> IssueWebhook([FromBody] GitHubIssueRequest request)
    {
        var repositories = await _context.GitHubRepositories
            .Where(r => r.OwnerName == request.RepositoryOwner && r.RepositoryName == request.RepositoryName)
            .ToListAsync();
            
        if (!repositories.Any())
            return NotFound();
            
        foreach (var repo in repositories)
        {
            await _webhookService.TriggerWebhook(
                repo.Email,
                WebhookType.GitHub,
                new
                {
                    repository = request.RepositoryName,
                    owner = request.RepositoryOwner,
                    issue = request.Issue
                });
        }
        
        return Ok();
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
