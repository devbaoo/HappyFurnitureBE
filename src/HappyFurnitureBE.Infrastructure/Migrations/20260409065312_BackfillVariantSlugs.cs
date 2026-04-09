using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyFurnitureBE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillVariantSlugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill slugs for existing variants that have no slug yet
            // Converts ColorName to a URL-friendly slug:
            // 1. Lowercase
            // 2. Remove special characters except spaces and hyphens
            // 3. Replace spaces with hyphens
            migrationBuilder.Sql(@"
                UPDATE ""ProductVariants""
                SET ""Slug"" = LOWER(
                    REGEXP_REPLACE(
                        REGEXP_REPLACE(
                            REGEXP_REPLACE(
                                ""ColorName"",
                                '[^a-zA-Z0-9\s-]', '', 'g'
                            ),
                            '\s+', '-', 'g'
                        ),
                        '-+', '-', 'g'
                    )
                )
                WHERE ""Slug"" IS NULL AND ""ColorName"" IS NOT NULL AND ""ColorName"" != '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Cannot reliably reverse a backfill
        }
    }
}
