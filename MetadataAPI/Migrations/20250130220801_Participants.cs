using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetadataAPI.Migrations
{
    /// <inheritdoc />
    public partial class Participants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonData",
                table: "ClinicalTrialMetadata");

            migrationBuilder.AlterColumn<int>(
                name: "Participants",
                table: "ClinicalTrialMetadata",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Participants",
                table: "ClinicalTrialMetadata",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "JsonData",
                table: "ClinicalTrialMetadata",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
