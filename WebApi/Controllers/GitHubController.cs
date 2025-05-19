using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext; // To access the database dbcontext
using MyAppDemo.DataLayer.Models; // To access the entity
using MyAppDemo.WebAPI.Models.Requests; // To access the DTO (request)
using MyAppDemo.WebAPI.Models.Response;
using MyAppDemo.WebAPI.Models.Responses;
using MyAppDemo.WebAPI.Services; // To access the service
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;

namespace MyAppDemo.WebAPI.Controllers;


/// <summary>
/// GitHub
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{

    private readonly IWebhookService _webhookService;
    private readonly WebAPIDbContext _dbContext;
    private readonly ILogger<GitHubController> _logger;

    public GitHubController(IWebhookService webhookService, WebAPIDbContext dbcontext, ILogger<GitHubController> logger)
    {
        _webhookService = webhookService;
        _dbContext = dbcontext;
        _logger = logger;
    }

    /// <summary>
    /// Register a GitHub repository.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register-repository")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
            Summary = "Register a GitHub repository",
            Description = "Register a GitHub repository associated with a GitHub user and create a subscription to set in the GitHub Webhook.",
            OperationId = "RegisterGitHubRepository"
        )]
    public async Task<IActionResult> RegisterRepository([FromBody] GitHubRepositoryRequest request)
    {

        // Check if it already exists
        var exists = await _dbContext.GitHubRepositories.AnyAsync(r =>
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

        _dbContext.GitHubRepositories.Add(repository);
        await _dbContext.SaveChangesAsync();


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
    /// Note: [AllowAnonymous] will not working because the API Key authentication middleware runs before the ASP.NET Core authorization pipeline, and it does not check whether the endpoint should be anonymous.
    /// </summary>
    /// <returns>HTTP response 200 with the edit data or 404 if the repository is not found.</returns>
    [HttpPost("issue-webhook")]
    [SwaggerOperation(
            Summary = "Receive a GitHub Issue via GitHub Webhook",
            Description = "Receives a GitHub Issue via GitHub Webhook.",
            OperationId = "GitHubIssue"
        )]
    public async Task<IActionResult> IssueWebhook()
    {
        _logger.LogInformation("IssueWebhook run");


        // Read the original request body as text
        Request.EnableBuffering();
        string body;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
        }
        Request.Body.Position = 0;


        // Extract JSON correctly depending on Content-Type
        string jsonBody = null;
        // Check the Content-Type
        if (Request.ContentType != null && Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = HttpUtility.ParseQueryString(body);
            jsonBody = parsed["payload"];
        }
        else if (Request.ContentType != null && Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
        {
            // Body is already pure JSON
            jsonBody = body;
        }
        else
        {
            return StatusCode(415, "Unsupported Content-Type");
        }

        if (string.IsNullOrWhiteSpace(jsonBody))
            return BadRequest("Payload is missing.");

        // Deserialize JSON to DTO Requests
        GitHubIssueRequest request;
        try
        {
            request = JsonConvert.DeserializeObject<GitHubIssueRequest>(jsonBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing payload");
            return BadRequest("Invalid payload format.");
        }


        _logger.LogInformation("Received GitHub webhook for issue: {Title}", request.Issue?.Title);


        // Check if the repository exists in the database
        var repositories = await _dbContext.GitHubRepositories
            .Where(r => r.OwnerName == request.Repository.Owner.Login && r.RepositoryName == request.Repository.Name)
            .Include(r => r.Webhook)
            .ToListAsync();

        if (!repositories.Any())
            return NotFound("Repository not found.");


        foreach (var repo in repositories)
        {

            // Validation is done in the middleware, not in the IssueWebhook method
            //var secret = repo.WebhookSecret;
            //if (!string.Equals(signature, $"sha256={computedSignature}", StringComparison.OrdinalIgnoreCase))
            //    continue; // invalid signature


            // 'Payload' is the data that will be sent to Power Automate
            var payload = new GitHubIssueResponse
            {
                Title = request.Issue.Title,
                Body = request.Issue.Body,
                Html_Url = request.Issue.Html_Url,
                User = request.Issue.User.Login,
                RepositoryName = request.Repository.Name,
                RepositoryOwner = request.Repository.Owner.Login
            };



            // Check or create the user
            var user = await _dbContext.GitHubUsers
             .FirstOrDefaultAsync(u => u.Login == payload.User);

            if (user == null)
            {
                user = new GitHubUser
                {
                    Login = payload.User,
                    AvatarUrl = request.Issue.User.Avatar_Url,
                    ProfileUrl = request.Issue.User.Html_Url
                };

                _dbContext.GitHubUsers.Add(user);
                await _dbContext.SaveChangesAsync();
            }


            // Check if the issue already exists to avoid duplicates
            var existingIssue = await _dbContext.GitHubIssues
             .FirstOrDefaultAsync(i => i.IssueNumber == request.Issue.Number && i.RepositoryId == repo.GitHubRepoId);

            if (existingIssue == null)
            {
                // Create the issue based on the payload
                var issue = new GitHubIssue
                {
                    IssueNumber = request.Issue.Number,
                    Title = payload.Title,
                    Body = payload.Body,
                    Html_Url = payload.Html_Url,
                    CreatedAt = request.Issue.Created_At,
                    UserLogin = payload.User,
                    RepositoryId = repo.GitHubRepoId,
                    UserId = user.GitHubUserId
                };

                _dbContext.GitHubIssues.Add(issue);
                await _dbContext.SaveChangesAsync();
            }

            var response = await _webhookService.TriggerWebhook(repo.Webhook.WebhookId, WebhookType.GitHub, payload);

            if (response)
            {
                return Ok(new { message = "Issue sent successfully." });
            }

            return StatusCode(500, new { message = "Failed to submit issue to webhook." });
        }

        //return NotFound("No webhook found for the repository.");
        return Ok(new { message = "No webhook found for the repository." });
    }

    /// <summary>
    /// Endpoint to register for Custom Connectors for GitHub.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("customconnector-webhook")]
    [SwaggerOperation(
            Summary = "Register the Webhook for GitHuB Custom Connectors.",
            Description = "Registers the Webhook for the GitHub Custom Connector that will receive the issue submitted to the repository.",
            OperationId = "AddCustomConnectorGitHubWebhook"
        )]
    [ProducesResponseType(typeof(WebhookResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterWebhook([FromBody] WebhookRegistrationRequest request)
    {


        try
        {

            // Search for the repository in the database
            var repository = await _dbContext.GitHubRepositories
            .FirstOrDefaultAsync(r =>
            r.RepositoryName == request.RepositoryName &&
            r.OwnerName == request.OwnerUsername);

            if (repository == null)
            {
                return NotFound(new { Message = "Repository not found." });
            }


            // Check if there is already a record in the database with the FlowId.
            var existingWebhook = await _dbContext.Webhooks
             .FirstOrDefaultAsync(w => w.FlowId == request.FlowId);

            if (existingWebhook != null)
            {
                // Only update the CallbackUrl
                existingWebhook.WebhookUrl = request.WebhookUrl;
                _dbContext.Webhooks.Update(existingWebhook);

                _logger.LogInformation("Webhook updated for FlowId: {FlowId}", request.FlowId);

                repository.WebhookId = existingWebhook.WebhookId;
            }
            else
            {
                // Register the webhook
                var webhook = await _webhookService.RegisterWebhook(
                request.Email,
                request.WebhookUrl,
                WebhookType.GitHub,
                request.FlowId);

                _logger.LogInformation("New webhook registered for FlowId: {FlowId}", request.FlowId);

                repository.WebhookId = webhook.WebhookId;
            }

            await _dbContext.SaveChangesAsync();

            var deleteUrl = GenerateDeleteUri(Request.Scheme, Request.Host.ToString(), request.FlowId);

            var obj = new WebhookResponse
            {
                Success = true,
                Message = "Power Automate webhook registered successfully.",
                FlowId = request.FlowId,
                Location = deleteUrl,
            };

            return Ok(obj);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering webhook for FlowId: {FlowId}", request.FlowId);
            return StatusCode(500, new { message = "Error registering webhook." });
        }
    }


    private static string GenerateDeleteUri(string scheme, string host, string flowId)
    {
        return $"{scheme}://{host}/api/github/webhook/{flowId}";
    }

    /// <summary>
    /// Endpoint to remove a Custom Connectors for GitHub.
    /// </summary>
    /// <param name="flowId">Flow identification name of your choice</param>
    /// <returns></returns>
    [HttpDelete("webhook/{flowId}")]
    [SwaggerOperation(
            Summary = "Remove the Webhook for GitHuB Custom Connectors.",
            Description = "Remove the Webhook for the GitHub Custom Connector that will receive the issue submitted to the repository.",
            OperationId = "RemoveCustomConnectorGitHubWebhook"
        )]
    public async Task<IActionResult> RemoveWebhook(string flowId)
    {
        var result = await _webhookService.RemoveWebhook(flowId);

        if (!result)
            return NotFound();

        return Ok(new { Message = "Custom Connectors Webhook for GitHub removed successfully" });
    }
}
