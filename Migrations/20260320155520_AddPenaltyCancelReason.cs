using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexFit.Migrations
{
    /// <inheritdoc />
    public partial class AddPenaltyCancelReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "PenaltyPoints",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                table: "PenaltyPoints",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "PenaltyCards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                table: "PenaltyCards",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "PenaltyPoints");

            migrationBuilder.DropColumn(
                name: "IsCanceled",
                table: "PenaltyPoints");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "PenaltyCards");

            migrationBuilder.DropColumn(
                name: "IsCanceled",
                table: "PenaltyCards");
        }
    }
}
