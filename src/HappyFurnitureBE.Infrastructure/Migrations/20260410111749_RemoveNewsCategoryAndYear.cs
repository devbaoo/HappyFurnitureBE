using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNewsCategoryAndYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "News");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "News",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "News",
                type: "integer",
                nullable: true);
        }
    }
}
