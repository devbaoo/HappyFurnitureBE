using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialsToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "happyfurniture",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "ProductVariants",
                schema: "happyfurniture",
                newName: "ProductVariants");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "happyfurniture",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "ProductImages",
                schema: "happyfurniture",
                newName: "ProductImages");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                schema: "happyfurniture",
                newName: "ProductCategories");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "happyfurniture",
                newName: "Categories");

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductMaterials",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMaterials", x => new { x.ProductId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_ProductMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMaterials_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMaterials_MaterialId",
                table: "ProductMaterials",
                column: "MaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMaterials");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.EnsureSchema(
                name: "happyfurniture");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "happyfurniture");

            migrationBuilder.RenameTable(
                name: "ProductVariants",
                newName: "ProductVariants",
                newSchema: "happyfurniture");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "happyfurniture");

            migrationBuilder.RenameTable(
                name: "ProductImages",
                newName: "ProductImages",
                newSchema: "happyfurniture");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "ProductCategories",
                newSchema: "happyfurniture");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Categories",
                newSchema: "happyfurniture");
        }
    }
}
