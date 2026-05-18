using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentBlockAlignmentAndColumn3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alignment",
                table: "ContentBlocks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content3En",
                table: "ContentBlocks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content3Vi",
                table: "ContentBlocks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image3AltEn",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image3AltVi",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image3Url",
                table: "ContentBlocks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title3En",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title3Vi",
                table: "ContentBlocks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alignment",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Content3En",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Content3Vi",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Image3AltEn",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Image3AltVi",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Image3Url",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Title3En",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Title3Vi",
                table: "ContentBlocks");
        }
    }
}
