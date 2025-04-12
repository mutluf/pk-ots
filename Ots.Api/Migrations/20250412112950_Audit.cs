using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ots.Api.Migrations.MsSql
{
    /// <inheritdoc />
    public partial class Audit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "dbo");
        }
    }
}
