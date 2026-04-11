using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewsTypeAndAddBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""News"" 
                ALTER COLUMN ""Type"" TYPE VARCHAR(20) USING ""Type""::VARCHAR,
                ALTER COLUMN ""Type"" SET DEFAULT 'News';
            ");

            migrationBuilder.Sql(@"
                UPDATE ""News"" SET ""Type"" = 'News' WHERE ""Type"" = 'Event';
                UPDATE ""News"" SET ""Type"" = 'CompanyActivity' WHERE ""Type"" = 'Activity';
            ");

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "News",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "News");

            migrationBuilder.Sql(@"
                UPDATE ""News"" SET ""Type"" = 'Event' WHERE ""Type"" = 'News';
                UPDATE ""News"" SET ""Type"" = 'Activity' WHERE ""Type"" = 'CompanyActivity';
            ");
        }
    }
}
