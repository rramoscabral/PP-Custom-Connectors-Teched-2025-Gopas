using MyAppDemo.DataLayer.Models; // To access the entity

namespace MyAppDemo.WebAPI.Services;

public interface IPerplexityService
{
    Task<object> GenerateContentAsync(string prompt, string email);
}