using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultilingualSupport_Material_Assembly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Materials",
                newName: "NameVi");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Materials",
                newName: "DescriptionVi");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Assemblies",
                newName: "NameVi");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Assemblies",
                newName: "DescriptionVi");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Materials",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Materials",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Assemblies",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Assemblies",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Assemblies");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Assemblies");

            migrationBuilder.RenameColumn(
                name: "NameVi",
                table: "Materials",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionVi",
                table: "Materials",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "NameVi",
                table: "Assemblies",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionVi",
                table: "Assemblies",
                newName: "Description");
        }
    }
}
