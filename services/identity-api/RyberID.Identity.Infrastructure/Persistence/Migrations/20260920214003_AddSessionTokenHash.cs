using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RyberID.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "token_hash",
                table: "sessions",
                type: "bytea",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE sessions
                SET token_hash = uuid_send(id) || uuid_send(id)
                WHERE token_hash IS NULL;
                """);

            migrationBuilder.AlterColumn<byte[]>(
                name: "token_hash",
                table: "sessions",
                type: "bytea",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessions_token_hash",
                table: "sessions",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sessions_token_hash",
                table: "sessions");

            migrationBuilder.DropColumn(
                name: "token_hash",
                table: "sessions");
        }
    }
}
