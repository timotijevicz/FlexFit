using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FlexFit.Migrations
{
    /// <inheritdoc />
    public partial class up : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipCards_Members_MemberId",
                table: "MembershipCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyCards_Members_MemberId",
                table: "PenaltyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyCards_Members_MemberId1",
                table: "PenaltyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyPoints_Members_MemberId",
                table: "PenaltyPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Members_MemberId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_PenaltyCards_MemberId1",
                table: "PenaltyCards");

            migrationBuilder.DropIndex(
                name: "IX_MembershipCards_MemberId",
                table: "MembershipCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MemberId1",
                table: "PenaltyCards");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "MembershipCards");

            migrationBuilder.RenameTable(
                name: "Members",
                newName: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "PenaltyPoints",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "JMBG",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeType",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Users",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipCards_CardNumber",
                table: "MembershipCards",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipCards_MemberId",
                table: "MembershipCards",
                column: "MemberId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipCards_Users_MemberId",
                table: "MembershipCards",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyCards_Users_MemberId",
                table: "PenaltyCards",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyPoints_Users_MemberId",
                table: "PenaltyPoints",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_MemberId",
                table: "Reservations",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipCards_Users_MemberId",
                table: "MembershipCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyCards_Users_MemberId",
                table: "PenaltyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyPoints_Users_MemberId",
                table: "PenaltyPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_MemberId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_MembershipCards_CardNumber",
                table: "MembershipCards");

            migrationBuilder.DropIndex(
                name: "IX_MembershipCards_MemberId",
                table: "MembershipCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmployeeType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "License",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Members");

            migrationBuilder.AddColumn<int>(
                name: "MemberId1",
                table: "PenaltyCards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "MembershipCards",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PenaltyPoints",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JMBG",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    EmployeeType = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    License = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyCards_MemberId1",
                table: "PenaltyCards",
                column: "MemberId1");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipCards_MemberId",
                table: "MembershipCards",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipCards_Members_MemberId",
                table: "MembershipCards",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyCards_Members_MemberId",
                table: "PenaltyCards",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyCards_Members_MemberId1",
                table: "PenaltyCards",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyPoints_Members_MemberId",
                table: "PenaltyPoints",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Members_MemberId",
                table: "Reservations",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
