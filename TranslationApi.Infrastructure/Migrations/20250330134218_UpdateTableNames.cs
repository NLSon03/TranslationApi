using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranslationApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_ChatSession_SessionId",
                table: "ChatMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSession_AIModel_AIModelId",
                table: "ChatSession");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSession_AspNetUsers_UserId",
                table: "ChatSession");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_ChatMessage_MessageId",
                table: "Feedback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatSession",
                table: "ChatSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage");

            migrationBuilder.RenameTable(
                name: "Feedback",
                newName: "Feedbacks");

            migrationBuilder.RenameTable(
                name: "ChatSession",
                newName: "ChatSessions");

            migrationBuilder.RenameTable(
                name: "ChatMessage",
                newName: "ChatMessages");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_MessageId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatSession_UserId",
                table: "ChatSessions",
                newName: "IX_ChatSessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatSession_AIModelId",
                table: "ChatSessions",
                newName: "IX_ChatSessions_AIModelId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessage_SessionId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatSessions",
                table: "ChatSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatSessions_SessionId",
                table: "ChatMessages",
                column: "SessionId",
                principalTable: "ChatSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_AIModel_AIModelId",
                table: "ChatSessions",
                column: "AIModelId",
                principalTable: "AIModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_AspNetUsers_UserId",
                table: "ChatSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_ChatMessages_MessageId",
                table: "Feedbacks",
                column: "MessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatSessions_SessionId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_AIModel_AIModelId",
                table: "ChatSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_AspNetUsers_UserId",
                table: "ChatSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_ChatMessages_MessageId",
                table: "Feedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatSessions",
                table: "ChatSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages");

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                newName: "Feedback");

            migrationBuilder.RenameTable(
                name: "ChatSessions",
                newName: "ChatSession");

            migrationBuilder.RenameTable(
                name: "ChatMessages",
                newName: "ChatMessage");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_MessageId",
                table: "Feedback",
                newName: "IX_Feedback_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatSessions_UserId",
                table: "ChatSession",
                newName: "IX_ChatSession_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatSessions_AIModelId",
                table: "ChatSession",
                newName: "IX_ChatSession_AIModelId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_SessionId",
                table: "ChatMessage",
                newName: "IX_ChatMessage_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatSession",
                table: "ChatSession",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_ChatSession_SessionId",
                table: "ChatMessage",
                column: "SessionId",
                principalTable: "ChatSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSession_AIModel_AIModelId",
                table: "ChatSession",
                column: "AIModelId",
                principalTable: "AIModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSession_AspNetUsers_UserId",
                table: "ChatSession",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_ChatMessage_MessageId",
                table: "Feedback",
                column: "MessageId",
                principalTable: "ChatMessage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
