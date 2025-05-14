using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAppDemo.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKeyToAuthorizedEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                schema: "CustomConnector",
                table: "AuthorizedEmails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                schema: "CustomConnector",
                table: "AuthorizedEmails");
        }
    }
}
