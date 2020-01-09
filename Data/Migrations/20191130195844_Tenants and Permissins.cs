using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Api.Data.Migrations
{
    public partial class TenantsandPermissins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionGuid",
                table: "Sessions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "Sessions",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // To change the IDENTITY property of a column, the column needs to be dropped and recreated.
            migrationBuilder.DropForeignKey("FK_Permissions_PermissionTypes_PermissionTypeId", "Permissions");
            migrationBuilder.DropPrimaryKey("PK_PermissionTypes", "PermissionTypes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PermissionTypes");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PermissionTypes",
                nullable: false);

            migrationBuilder.AddPrimaryKey("PK_PermissionTypes", "PermissionTypes", "Id");
            migrationBuilder.AddForeignKey("FK_Permissions_PermissionTypes_PermissionTypeId", "Permissions", "PermissionTypeId", "PermissionTypes", principalColumn: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceType",
                table: "AuditLogs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Change",
                table: "AuditLogs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SessionGuid",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "Sessions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            // To change the IDENTITY property of a column, the column needs to be dropped and recreated.
            migrationBuilder.DropForeignKey("FK_Permissions_PermissionTypes_PermissionTypeId", "Permissions");
            migrationBuilder.DropPrimaryKey("PK_PermissionTypes", "PermissionTypes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PermissionTypes");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PermissionTypes",
                nullable: false);

            migrationBuilder.AddPrimaryKey("PK_PermissionTypes", "PermissionTypes", "Id");
            migrationBuilder.AddForeignKey("FK_Permissions_PermissionTypes_PermissionTypeId", "Permissions", "PermissionTypeId", "PermissionTypes", principalColumn: "Id");


            migrationBuilder.AlterColumn<string>(
                name: "ReferenceType",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Change",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
