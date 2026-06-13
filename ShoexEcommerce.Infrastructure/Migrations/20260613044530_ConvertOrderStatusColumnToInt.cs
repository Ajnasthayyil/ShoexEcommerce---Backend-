using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SheoxEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertOrderStatusColumnToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Orders' 
      AND COLUMN_NAME = 'Status' 
      AND DATA_TYPE LIKE '%char%'
)
BEGIN
    EXEC sp_executesql N'
        UPDATE Orders SET Status = ''0'' WHERE Status = ''PendingPayment'';
        UPDATE Orders SET Status = ''1'' WHERE Status = ''Ordered'';
        UPDATE Orders SET Status = ''2'' WHERE Status = ''UnderProcess'';
        UPDATE Orders SET Status = ''3'' WHERE Status = ''Packed'';
        UPDATE Orders SET Status = ''4'' WHERE Status = ''Shipped'';
        UPDATE Orders SET Status = ''5'' WHERE Status = ''Delivered'';
        UPDATE Orders SET Status = ''6'' WHERE Status = ''Cancelled'';
        UPDATE Orders SET Status = ''1'' WHERE Status NOT IN (''0'', ''1'', ''2'', ''3'', ''4'', ''5'', ''6'');
    ';
END
");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Orders' 
      AND COLUMN_NAME = 'Status' 
      AND DATA_TYPE LIKE '%char%'
)
BEGIN
    EXEC sp_executesql N'
        UPDATE Orders SET Status = ''PendingPayment'' WHERE Status = ''0'';
        UPDATE Orders SET Status = ''Ordered'' WHERE Status = ''1'';
        UPDATE Orders SET Status = ''UnderProcess'' WHERE Status = ''2'';
        UPDATE Orders SET Status = ''Packed'' WHERE Status = ''3'';
        UPDATE Orders SET Status = ''Shipped'' WHERE Status = ''4'';
        UPDATE Orders SET Status = ''Delivered'' WHERE Status = ''5'';
        UPDATE Orders SET Status = ''Cancelled'' WHERE Status = ''6'';
    ';
END
");
        }
    }
}


