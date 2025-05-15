using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAppDemo.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GitHubRepositories_OwnerName_RepositoryName_Email",
                schema: "CustomConnector",
                table: "GitHubRepositories");

            migrationBuilder.CreateTable(
                name: "GitHubUsers",
                schema: "CustomConnector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProfileUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GitHubIssues",
                schema: "CustomConnector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Html_Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GitHubIssues_GitHubRepositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalSchema: "CustomConnector",
                        principalTable: "GitHubRepositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GitHubIssues_GitHubUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "CustomConnector",
                        principalTable: "GitHubUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_OwnerName_RepositoryName_WebhookSecret_Email",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                columns: new[] { "OwnerName", "RepositoryName", "WebhookSecret", "Email" },
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GitHubIssues",
                schema: "CustomConnector");

            migrationBuilder.DropTable(
                name: "GitHubUsers",
                schema: "CustomConnector");

            migrationBuilder.DropIndex(
                name: "IX_GitHubRepositories_OwnerName_RepositoryName_WebhookSecret_Email",
                schema: "CustomConnector",
                table: "GitHubRepositories");

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_OwnerName_RepositoryName_Email",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                columns: new[] { "OwnerName", "RepositoryName", "Email" },
                unique: true);
        }
    }
}
