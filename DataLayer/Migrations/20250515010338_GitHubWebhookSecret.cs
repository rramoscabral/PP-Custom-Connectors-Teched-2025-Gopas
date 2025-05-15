using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAppDemo.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class GitHubWebhookSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebhookSecret",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebhookSecret",
                schema: "CustomConnector",
                table: "GitHubRepositories");
        }
    }
}
