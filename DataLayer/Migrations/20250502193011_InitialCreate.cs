using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAppDemo.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CustomConnector");

            migrationBuilder.CreateTable(
                name: "AuthorizedEmails",
                schema: "CustomConnector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Service = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedEmails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GitHubRepositories",
                schema: "CustomConnector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RepositoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubRepositories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Webhooks",
                schema: "CustomConnector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WebhookUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlowId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Webhooks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedEmails_Email_Service",
                schema: "CustomConnector",
                table: "AuthorizedEmails",
                columns: new[] { "Email", "Service" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_OwnerName_RepositoryName_Email",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                columns: new[] { "OwnerName", "RepositoryName", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Webhooks_WebhookUrl",
                schema: "CustomConnector",
                table: "Webhooks",
                column: "WebhookUrl",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizedEmails",
                schema: "CustomConnector");

            migrationBuilder.DropTable(
                name: "GitHubRepositories",
                schema: "CustomConnector");

            migrationBuilder.DropTable(
                name: "Webhooks",
                schema: "CustomConnector");
        }
    }
}
