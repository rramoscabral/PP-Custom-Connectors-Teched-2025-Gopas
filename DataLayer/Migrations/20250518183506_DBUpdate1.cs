using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAppDemo.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class DBUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_Webhooks_WebhookId",
                schema: "CustomConnector",
                table: "GitHubRepositories");

            migrationBuilder.AlterColumn<int>(
                name: "WebhookId",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_Webhooks_WebhookId",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                column: "WebhookId",
                principalSchema: "CustomConnector",
                principalTable: "Webhooks",
                principalColumn: "WebhookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_Webhooks_WebhookId",
                schema: "CustomConnector",
                table: "GitHubRepositories");

            migrationBuilder.AlterColumn<int>(
                name: "WebhookId",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_Webhooks_WebhookId",
                schema: "CustomConnector",
                table: "GitHubRepositories",
                column: "WebhookId",
                principalSchema: "CustomConnector",
                principalTable: "Webhooks",
                principalColumn: "WebhookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
