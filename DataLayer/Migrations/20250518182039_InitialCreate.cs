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
                    AuthorizedEmailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Service = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedEmails", x => x.AuthorizedEmailId);
                });

            migrationBuilder.CreateTable(
                name: "GitHubUsers",
                schema: "CustomConnector",
                columns: table => new
                {
                    GitHubUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProfileUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubUsers", x => x.GitHubUserId);
                });

            migrationBuilder.CreateTable(
                name: "Webhooks",
                schema: "CustomConnector",
                columns: table => new
                {
                    WebhookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WebhookUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlowId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastTrigger = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Webhooks", x => x.WebhookId);
                });

            migrationBuilder.CreateTable(
                name: "GitHubRepositories",
                schema: "CustomConnector",
                columns: table => new
                {
                    GitHubRepoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RepositoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WebhookSecret = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WebhookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubRepositories", x => x.GitHubRepoId);
                    table.ForeignKey(
                        name: "FK_GitHubRepositories_Webhooks_WebhookId",
                        column: x => x.WebhookId,
                        principalSchema: "CustomConnector",
                        principalTable: "Webhooks",
                        principalColumn: "WebhookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GitHubIssues",
                schema: "CustomConnector",
                columns: table => new
                {
                    GitHubIssueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Html_Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubIssues", x => x.GitHubIssueId);
                    table.ForeignKey(
                        name: "FK_GitHubIssues_GitHubRepositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalSchema: "CustomConnector",
                        principalTable: "GitHubRepositories",
                        principalColumn: "GitHubRepoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GitHubIssues_GitHubUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "CustomConnector",
                        principalTable: "GitHubUsers",
                        principalColumn: "GitHubUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedEmails_Email_Service",
                schema: "CustomConnector",
                table: "AuthorizedEmails",
                columns: new[] { "Email", "Service" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubIssues_RepositoryId",
                schema: "CustomConnector",
                table: "GitHubIssues",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHubIssues_UserId",
                schema: "CustomConnector",
                table: "GitHubIssues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_OwnerName_RepositoryName_WebhookSecret_Email",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                columns: new[] { "OwnerName", "RepositoryName", "WebhookSecret", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_WebhookId",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                column: "WebhookId");

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
                name: "GitHubIssues",
                schema: "CustomConnector");

            migrationBuilder.DropTable(
                name: "GitHubRepositories",
                schema: "CustomConnector");

            migrationBuilder.DropTable(
                name: "GitHubUsers",
                schema: "CustomConnector");

            migrationBuilder.DropTable(
                name: "Webhooks",
                schema: "CustomConnector");
        }
    }
}
