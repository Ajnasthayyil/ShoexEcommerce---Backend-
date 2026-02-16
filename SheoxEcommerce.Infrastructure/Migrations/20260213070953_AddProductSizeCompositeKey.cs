using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SheoxEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSizeCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SizeId1",
                table: "ProductSizes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_SizeId1",
                table: "ProductSizes",
                column: "SizeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_Sizes_SizeId1",
                table: "ProductSizes",
                column: "SizeId1",
                principalTable: "Sizes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_Sizes_SizeId1",
                table: "ProductSizes");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizes_SizeId1",
                table: "ProductSizes");

            migrationBuilder.DropColumn(
                name: "SizeId1",
                table: "ProductSizes");
        }
    }
}
