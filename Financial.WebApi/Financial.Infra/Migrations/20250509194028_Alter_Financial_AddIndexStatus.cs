using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Financial_AddIndexStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Financiallaunch_Status",
                table: "Financiallaunch",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Financiallaunch_Status",
                table: "Financiallaunch");
        }
    }
}
