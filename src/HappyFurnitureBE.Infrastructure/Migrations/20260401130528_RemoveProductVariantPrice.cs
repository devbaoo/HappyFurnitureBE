using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductVariantPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductVariants",
                type: "numeric(18,2)",
                nullable: true);
        }
    }
}
