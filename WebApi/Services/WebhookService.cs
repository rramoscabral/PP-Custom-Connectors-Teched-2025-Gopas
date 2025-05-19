using Azure;
using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext;
using MyAppDemo.DataLayer.Models; // To access the entity
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;



namespace MyAppDemo.WebAPI.Services;

/// <summary>
/// Generic webhook management service for multiple types of webhooks with Single Responsibility Principle (SRP).
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
    /// <param name="type">Webhook type PowerAutomate, GitHub, or Perplexity</param>
    /// <param name="flowId"></param>
    /// <returns></returns>
    public async Task<Webhook> RegisterWebhook(string email, string webhookUrl, WebhookType type, string flowId)
    {
        var webhook = new Webhook
        {
            Email = email,
            WebhookUrl = webhookUrl,
            Type = type,
            FlowId = flowId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Webhooks.Add(webhook);
        await _context.SaveChangesAsync();

        return webhook;
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
    /// <param name="webhookID">Webook record ID from database</param>
    /// <param name="type">Webhook type PowerAutomate, GitHub, or Perplexity</param>
    /// <param name="payload">Payload</param>
    /// <returns></returns>
    public async Task<bool> TriggerWebhook(int webhookID, WebhookType type, object payload)
    {
        try
        {
            var webhook = await _context.Webhooks.FindAsync(webhookID);
            if (webhook != null)
            {
                var content = new StringContent(
                          JsonSerializer.Serialize(payload),
                          Encoding.UTF8,
                          "application/json");


                await _httpClient.PostAsync(webhook.WebhookUrl, content);

                _logger.LogInformation("Sending payload to {Url}: {Payload}", webhook.WebhookUrl, content);

                // Update the LastTrigger field
                webhook.LastTrigger = DateTime.UtcNow;

                // Save all changes at once
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Webhook with ID {WebhookID} not found in database.", webhookID);
                return false;
            }

        }
        catch (Exception ex)
        {
            // Log the error or take some action
            _logger.LogError(ex, $"Error sending webhook.");

            //throw new Exception($"Error sending webhook to: {webhookURL}");
            return false;
        }
        return true;
    }
}