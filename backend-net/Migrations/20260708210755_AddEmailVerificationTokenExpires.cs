using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_net.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationTokenExpires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "email_verification_token_expires",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email_verification_token_expires",
                table: "users");
        }
    }
}
