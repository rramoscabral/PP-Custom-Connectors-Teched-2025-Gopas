﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
using MyAppDemo.WebAPI.Models.Requests; // To access the DTO (request)
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity
using MyAppDemo.WebAPI.Services; // To access the service

namespace MyAppDemo.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerplexityController : ControllerBase
{
    private readonly IWebhookService _webhookService;
    private readonly WebAPIDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public PerplexityController(IWebhookService webhookService, WebAPIDbContext context, HttpClient httpClient, IConfiguration configuration)
    {
        _webhookService = webhookService;
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateContent([FromBody] PerplexityRequest request)
    {
        // Verificar se o e-mail está autorizado
        var isAuthorized = await _context.AuthorizedEmails
            .AnyAsync(a => a.Email == request.Email && a.Service == ServiceType.Perplexity);
            
        if (!isAuthorized)
            return Unauthorized(new { Message = "Email not authorized for Perplexity service" });
            
        // Chamar a API do Perplexity
        var perplexityApiKey = _configuration["PerplexityApiKey"];
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {perplexityApiKey}");
        
        var perplexityRequest = new
        {
            model = "mixtral-8x7b-instruct",
            prompt = request.Prompt,
            max_tokens = 1024
        };
        
        var content = new StringContent(
            JsonConvert.SerializeObject(perplexityRequest),
            Encoding.UTF8,
            "application/json");
            
        var response = await _httpClient.PostAsync(
            "https://api.perplexity.ai/chat/completions",
            content);
            
        var responseContent = await response.Content.ReadAsStringAsync();
        
        return Ok(JsonConvert.DeserializeObject<object>(responseContent));
    }
    
    /// <summary>
    /// Endpoint to register a Custom Connectors for Perplexity.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("webhook")]
    public async Task<IActionResult> RegisterWebhook([FromBody] WebhookRegistrationRequest request)
    {
        await _webhookService.RegisterWebhook(
            request.Email,
            request.WebhookUrl,
            WebhookType.Perplexity,
            request.FlowId);
            
        return Ok(new { Message = "Custom Connectors Webhook registered successfully" });
    }
    
    /// <summary>
    /// Endpoint to remove a Custom Connectors for Perplexity.
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
