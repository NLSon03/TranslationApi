using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranslationApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModelTypeAndProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelType",
                table: "AIModel",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "AIModel",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelType",
                table: "AIModel");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "AIModel");
        }
    }
}
