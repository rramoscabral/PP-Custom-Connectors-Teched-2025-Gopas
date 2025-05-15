using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyAppDemo.WebAPI.Models.Requests; // To access the DTO (request)
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity
using MyAppDemo.WebAPI.Services; // To access the service
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography.Xml;
using System.Reflection.Metadata;
using System.Reflection.Emit;
using System.Web;

namespace MyAppDemo.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{

    private readonly IWebhookService _webhookService;
    private readonly WebAPIDbContext _context;
    private readonly ILogger<GitHubController> _logger;

    public GitHubController(IWebhookService webhookService, WebAPIDbContext context, ILogger<GitHubController> logger)
    {
        _webhookService = webhookService;
        _context = context;
        _logger = logger;
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
    /// Note: [AllowAnonymous] will not working because the API Key authentication middleware runs before the ASP.NET Core authorization pipeline, and it does not check whether the endpoint should be anonymous.
    /// </summary>
    /// <param name="request">object containing the GitHub issue, repository and user data.</param>
    /// <returns>HTTP response 200 with the edit data or 404 if the repository is not found.</returns>
    [HttpPost("issue-webhook")]
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
        var repositories = await _context.GitHubRepositories
            .Where(r => r.OwnerName == request.Repository.Owner.Login && r.RepositoryName == request.Repository.Name)
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
            var payload = new
            {
                title = request.Issue.Title,
                body = request.Issue.Body,
                html_url = request.Issue.Html_Url,
                user = request.Issue.User.Login,
                repository = request.Repository.Name,
                owner = request.Repository.Owner.Login
            };



            // Check or create the user
            var user = await _context.GitHubUsers
             .FirstOrDefaultAsync(u => u.Login == payload.user);

            if (user == null)
            {
                user = new GitHubUser
                {
                    Login = payload.user,
                    AvatarUrl = request.Issue.User.Avatar_Url,
                    ProfileUrl = request.Issue.User.Html_Url
                };

                _context.GitHubUsers.Add(user);
                await _context.SaveChangesAsync();
            }


            // Check if the issue already exists to avoid duplicates
            var existingIssue = await _context.GitHubIssues
             .FirstOrDefaultAsync(i => i.IssueNumber == request.Issue.Number && i.RepositoryId == repo.Id);

            if (existingIssue == null)
            {
                // Create the issue based on the payload
                var issue = new GitHubIssue
                {
                    IssueNumber = request.Issue.Number,
                    Title = payload.title,
                    Body = payload.body,
                    Html_Url = payload.html_url,
                    CreatedAt = request.Issue.Created_At,
                    UserLogin = payload.user,
                    RepositoryId = repo.Id,
                    UserId = user.Id
                };

                _context.GitHubIssues.Add(issue);
                await _context.SaveChangesAsync();
            }


            var webhook = await _context.Webhooks
                .FirstOrDefaultAsync(w => w.Email == repo.Email && w.Type == WebhookType.GitHub);

            if (webhook == null)
                continue;

            await _webhookService.TriggerWebhook(webhook.WebhookUrl, WebhookType.GitHub, payload);

            // Return the payload to Power Automate
            return Ok(payload);
        }

        return NotFound("No webhook found for the repository.");
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
