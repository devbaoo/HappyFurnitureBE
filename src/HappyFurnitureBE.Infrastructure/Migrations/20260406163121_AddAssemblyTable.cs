using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssemblyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssemblyId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Assemblies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assemblies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_AssemblyId",
                table: "Products",
                column: "AssemblyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Assemblies_AssemblyId",
                table: "Products",
                column: "AssemblyId",
                principalTable: "Assemblies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Assemblies_AssemblyId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Assemblies");

            migrationBuilder.DropIndex(
                name: "IX_Products_AssemblyId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AssemblyId",
                table: "Products");
        }
    }
}
