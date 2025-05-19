using MyAppDemo.DataLayer.Models; // To access the entity

namespace MyAppDemo.WebAPI.Services;

/// <summary>
/// Service for managing webhooks.
/// </summary>
public interface IWebhookService
{
    /// <summary>
    /// Registers a webhook.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="webhookUrl"></param>
    /// <param name="type"></param>
    /// <param name="flowId"></param>
    /// <returns></returns>
    Task<Webhook> RegisterWebhook(string email, string webhookUrl, WebhookType type, string flowId);

    /// <summary>
    /// Removes a webhook based on the flowId.
    /// </summary>
    /// <param name="flowId"></param>
    /// <returns></returns>
    Task<bool> RemoveWebhook(string flowId);

    /// <summary>
    /// Triggers a webhook by sending a POST request to the webhook Callback URL with the specified payload.
    /// </summary>
    /// <param name="webhookID"></param>
    /// <param name="type"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    Task<bool> TriggerWebhook(int webhookID, WebhookType type, object payload);
}
