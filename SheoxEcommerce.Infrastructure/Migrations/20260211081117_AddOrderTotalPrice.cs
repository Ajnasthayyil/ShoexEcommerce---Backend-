using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SheoxEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTotalPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Total",
                table: "OrderItems",
                newName: "TotalPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "OrderItems",
                newName: "Total");
        }
    }
}
