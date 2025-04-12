using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ots.Api.Migrations.MsSql
{
    /// <inheritdoc />
    public partial class Audit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChangedValues",
                schema: "dbo",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalValues",
                schema: "dbo",
                table: "AuditLog",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedValues",
                schema: "dbo",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "OriginalValues",
                schema: "dbo",
                table: "AuditLog");
        }
    }
}
