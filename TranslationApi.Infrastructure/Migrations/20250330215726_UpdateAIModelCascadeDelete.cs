using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranslationApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAIModelCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_AIModel_AIModelId",
                table: "ChatSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_AIModel_AIModelId",
                table: "ChatSessions",
                column: "AIModelId",
                principalTable: "AIModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_AIModel_AIModelId",
                table: "ChatSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_AIModel_AIModelId",
                table: "ChatSessions",
                column: "AIModelId",
                principalTable: "AIModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
