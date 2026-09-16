using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RyberID.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPasskeyAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "passkey_credentials",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credential_id = table.Column<byte[]>(type: "bytea", nullable: false),
                    public_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    sign_count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passkey_credentials", x => x.id);
                    table.ForeignKey(
                        name: "FK_passkey_credentials_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "passkey_user_handles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passkey_user_handles", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_passkey_user_handles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_passkey_credentials_credential_id",
                table: "passkey_credentials",
                column: "credential_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_passkey_credentials_user_id",
                table: "passkey_credentials",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_passkey_user_handles_value",
                table: "passkey_user_handles",
                column: "value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "passkey_credentials");

            migrationBuilder.DropTable(
                name: "passkey_user_handles");
        }
    }
}
