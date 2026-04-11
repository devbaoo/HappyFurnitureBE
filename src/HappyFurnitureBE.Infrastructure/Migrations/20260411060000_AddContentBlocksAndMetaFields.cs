using System;
using HappyFurnitureBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentBlocksAndMetaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionEn",
                table: "News",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionVi",
                table: "News",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleEn",
                table: "News",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleVi",
                table: "News",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContentBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NewsId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TitleVi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TitleEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentVi = table.Column<string>(type: "text", nullable: true),
                    ContentEn = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageAltVi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ImageAltEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFullWidth = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentBlocks_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_NewsId",
                table: "ContentBlocks",
                column: "NewsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionEn",
                table: "News");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionVi",
                table: "News");

            migrationBuilder.DropColumn(
                name: "MetaTitleEn",
                table: "News");

            migrationBuilder.DropColumn(
                name: "MetaTitleVi",
                table: "News");
        }
    }
}
