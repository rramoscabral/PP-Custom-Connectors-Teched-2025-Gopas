using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace MyAppDemo.DataLayer.DBContext
{
    public class WebAPIDbContextFactory : IDesignTimeDbContextFactory<WebAPIDbContext>
    {
        public WebAPIDbContext CreateDbContext(string[] args)
        {

            var connectionString = Environment.GetEnvironmentVariable("MSPPWebAPI2025");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Load settings from appsettings.json of WebAPI project
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "WebApi");
        
                IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

                connectionString = configuration.GetConnectionString("MSPPWebAPI2025");
            }

            
            var optionsBuilder = new DbContextOptionsBuilder<WebAPIDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new WebAPIDbContext(optionsBuilder.Options);
        }
    }
}