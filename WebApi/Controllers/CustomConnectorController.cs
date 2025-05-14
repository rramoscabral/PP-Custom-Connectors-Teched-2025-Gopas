using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyAppDemo.WebAPI.Models.Requests; // To access the DTO (request)
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity
using MyAppDemo.WebAPI.Services; // To access the service

namespace MyAppDemo.WebAPI.Controllers;


// WebAPI/Controllers/WebhookController.cs

[ApiController]
[Route("api/webhook")]
public class CustomConnectorController : ControllerBase
{

    private readonly IWebhookService _webhookService;

    private readonly WebAPIDbContext _context;

     public CustomConnectorController(IWebhookService webhookService, WebAPIDbContext context)
    {
        _webhookService = webhookService;
        _context = context;
    }
   
     [HttpPost("webhook")]
    public async Task<IActionResult> ReceiveWebhook([FromBody] WebhookRegistrationRequest request)
    {
        await _webhookService.RegisterWebhook(
            request.Email,
            request.WebhookUrl,
            WebhookType.PowerAutomate,
            request.FlowId);
            
        return Ok(new { Message = "Custom Connectors Webhook registered successfully" });
    }
    
    [HttpDelete("webhook/{flowId}")]
    public async Task<IActionResult> RemoveWebhook(string flowId)
    {
        var result = await _webhookService.RemoveWebhook(flowId);
        
        if (!result)
            return NotFound();
            
        return Ok(new { Message = "Custom Connectors Webhook removed successfully" });
    }

}
