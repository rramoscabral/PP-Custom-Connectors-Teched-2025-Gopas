using MyAppDemo.DataLayer.Models; // To access the entity

namespace MyAppDemo.WebAPI.Services;

public interface IWebhookService
{
    Task<bool> RegisterWebhook(string email, string webhookUrl, WebhookType type, string flowId);
    Task<bool> RemoveWebhook(string flowId);
    Task<bool> TriggerWebhook(string email, WebhookType type, object payload);
}
