using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext; // To access the database context
using MyAppDemo.DataLayer.Models; // To access the entity

namespace MyAppDemo.WebAPI.Services;

public class PerplexityService : IPerplexityService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public PerplexityService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<object> GenerateContentAsync(string prompt, string email)
    {
        var apiKey = _configuration["PerplexityApiKey"];
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var request = new
        {
            model = "mixtral-8x7b-instruct",
            prompt = prompt,
            max_tokens = 1024
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("https://api.perplexity.ai/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        // Warning: Null is not garanteed. The warning are suppressed by the compiler by using operator '!'.
        return JsonConvert.DeserializeObject<object>(responseContent)!;

    }
}
