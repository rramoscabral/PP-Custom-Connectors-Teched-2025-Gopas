using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext;
using MyAppDemo.DataLayer.Models; // To access the entity



namespace MyAppDemo.WebAPI.Services;


public class WebhookService : IWebhookService
{
    private readonly WebAPIDbContext _context;
    private readonly HttpClient _httpClient;
    
    public WebhookService(WebAPIDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }
    
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
    
    public async Task<bool> RemoveWebhook(string flowId)
    {
        var webhook = await _context.Webhooks.FirstOrDefaultAsync(w => w.FlowId == flowId);
        
        if (webhook == null)
            return false;
            
        _context.Webhooks.Remove(webhook);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
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
                
            await _httpClient.PostAsync(webhook.WebhookUrl, content);
        }
        
        return true;
    }
}