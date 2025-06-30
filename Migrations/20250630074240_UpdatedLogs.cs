using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QueryString",
                table: "RequestLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestBody",
                table: "RequestLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponseBody",
                table: "RequestLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "RequestLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueryString",
                table: "RequestLogs");

            migrationBuilder.DropColumn(
                name: "RequestBody",
                table: "RequestLogs");

            migrationBuilder.DropColumn(
                name: "ResponseBody",
                table: "RequestLogs");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "RequestLogs");
        }
    }
}
