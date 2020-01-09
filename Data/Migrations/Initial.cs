using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Api.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    ReferenceId = table.Column<int>(nullable: false),
                    ReferenceType = table.Column<string>(nullable: true),
                    Change = table.Column<string>(nullable: true),
                    FieldName = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    RoleId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    VerificationCode = table.Column<string>(nullable: true),
                    ConfirmEmailExpiry = table.Column<DateTime>(nullable: true),
                    LockoutEndDateUtc = table.Column<DateTime>(nullable: true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ForcePasswordReset = table.Column<bool>(nullable: false),
                    FacebookId = table.Column<string>(nullable: true),
                    GoogleId = table.Column<string>(nullable: true),
                    MicrosoftId = table.Column<string>(nullable: true),
                    LoginTypeId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    AuthenticationToken = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
