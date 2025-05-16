using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext;
using MyAppDemo.DataLayer.Models; // To access the entity



namespace MyAppDemo.WebAPI.Services;

/// <summary>
/// Service for managing webhooks.
/// </summary>
public class WebhookService : IWebhookService
{
    private readonly WebAPIDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(WebAPIDbContext context, HttpClient httpClient, ILogger<WebhookService> logger)
    {
        _context = context;
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Registers a webhook.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="webhookUrl">Webhook Callback URL</param>
    /// <param name="type">Webhook type PowerAutomate, GitHub, or Perplexity<</param>
    /// <param name="flowId"></param>
    /// <returns></returns>
    public async Task<bool> RegisterWebhook(string email, string webhookUrl, WebhookType type, string flowId)
    {
        var webhook = new Webhook
        {
            Email = email,
            WebhookUrl = webhookUrl,
            Type = type,
            FlowId = flowId
        };

        _context.Webhooks.Add(webhook);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Removes a webhook based on the flowId.
    /// </summary>
    /// <param name="flowId"></param>
    /// <returns></returns>
    public async Task<bool> RemoveWebhook(string flowId)
    {
        var webhook = await _context.Webhooks.FirstOrDefaultAsync(w => w.FlowId == flowId);

        if (webhook == null)
            return false;

        _context.Webhooks.Remove(webhook);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Triggers the webhook.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="type">Webhook type PowerAutomate, GitHub, or Perplexity</param>
    /// <param name="payload">Payload</param>
    /// <returns></returns>
    public async Task<bool> TriggerWebhook(string email, WebhookType type, object payload)
    {
        var webhooks = await _context.Webhooks
            .Where(w => w.Email == email && w.Type == type)
            .ToListAsync();

        if (!webhooks.Any())
            return false;

        foreach (var webhook in webhooks)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");


            try
            {

                await _httpClient.PostAsync(webhook.WebhookUrl, content);


                // Update the LastTrigger field
                webhook.LastTrigger = DateTime.Now;

                // Save all changes at once
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Logar o erro ou tomar alguma ação
                _logger.LogError(ex, $"Error sending webhook to {webhook.WebhookUrl}");
            }
        }

        return true;
    }
}