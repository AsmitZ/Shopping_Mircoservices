using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mango.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Category", "Description", "ImageURL", "Name", "Price" },
                values: new object[,]
                {
                    { 1L, "Appetizer", "Punjabi Samosa", "", "Samosa", 15m },
                    { 2L, "Appetizer", "Paneer Tikka with Marination", "", "Paneer Tikka", 20m },
                    { 3L, "Appetizer", "Mumbai Pav Bhaji", "", "Pav Bhaji", 10m },
                    { 4L, "Appetizer", "Mumbai Pani Puri", "", "Pani Puri", 5m },
                    { 5L, "Main Course", "South Indian Masala Dosa", "", "Masala Dosa", 10m },
                    { 6L, "Main Course", "Hyderabadi Chicken Biryani", "", "Chicken Biryani", 30m },
                    { 7L, "Main Course", "Bombay Veg Biryani", "", "Veg Biryani", 15m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
