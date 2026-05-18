using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewNewsBlockTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content2En",
                table: "ContentBlocks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content2Vi",
                table: "ContentBlocks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image2AltEn",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image2AltVi",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image2Url",
                table: "ContentBlocks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title2En",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title2Vi",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content2En",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Content2Vi",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Image2AltEn",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Image2AltVi",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Image2Url",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Title2En",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Title2Vi",
                table: "ContentBlocks");
        }
    }
}
