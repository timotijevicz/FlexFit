using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexFit.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "MembershipCards");

            migrationBuilder.AddColumn<int>(
                name: "MemberId1",
                table: "PenaltyCards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "MembershipCards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyCards_MemberId1",
                table: "PenaltyCards",
                column: "MemberId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyCards_Members_MemberId1",
                table: "PenaltyCards",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyCards_Members_MemberId1",
                table: "PenaltyCards");

            migrationBuilder.DropIndex(
                name: "IX_PenaltyCards_MemberId1",
                table: "PenaltyCards");

            migrationBuilder.DropColumn(
                name: "MemberId1",
                table: "PenaltyCards");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "MembershipCards");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "MembershipCards",
                type: "text",
                nullable: true);
        }
    }
}
