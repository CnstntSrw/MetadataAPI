using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetadataAPI.Migrations
{
    /// <inheritdoc />
    public partial class DurationInDays_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "ClinicalTrialMetadata",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "ClinicalTrialMetadata");
        }
    }
}
