using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RyberID.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sessions_user_id",
                table: "sessions",
                column: "user_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_sessions_token_hash_length",
                table: "sessions",
                sql: "octet_length(token_hash) = 32");

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_users_user_id",
                table: "sessions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sessions_users_user_id",
                table: "sessions");

            migrationBuilder.DropIndex(
                name: "IX_sessions_user_id",
                table: "sessions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_sessions_token_hash_length",
                table: "sessions");
        }
    }
}
