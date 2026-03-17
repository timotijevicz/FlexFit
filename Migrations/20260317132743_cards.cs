using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexFit.Migrations
{
    /// <inheritdoc />
    public partial class cards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipCards_FitnessObjects_FitnessObjectId",
                table: "MembershipCards");

            migrationBuilder.DropIndex(
                name: "IX_MembershipCards_FitnessObjectId",
                table: "MembershipCards");

            migrationBuilder.DropColumn(
                name: "FitnessObjectId",
                table: "MembershipCards");

            migrationBuilder.CreateTable(
                name: "DailyCardFitnessObject",
                columns: table => new
                {
                    DailyCardsId = table.Column<int>(type: "integer", nullable: false),
                    FitnessObjectsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyCardFitnessObject", x => new { x.DailyCardsId, x.FitnessObjectsId });
                    table.ForeignKey(
                        name: "FK_DailyCardFitnessObject_FitnessObjects_FitnessObjectsId",
                        column: x => x.FitnessObjectsId,
                        principalTable: "FitnessObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyCardFitnessObject_MembershipCards_DailyCardsId",
                        column: x => x.DailyCardsId,
                        principalTable: "MembershipCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyCardFitnessObject_FitnessObjectsId",
                table: "DailyCardFitnessObject",
                column: "FitnessObjectsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyCardFitnessObject");

            migrationBuilder.AddColumn<int>(
                name: "FitnessObjectId",
                table: "MembershipCards",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipCards_FitnessObjectId",
                table: "MembershipCards",
                column: "FitnessObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipCards_FitnessObjects_FitnessObjectId",
                table: "MembershipCards",
                column: "FitnessObjectId",
                principalTable: "FitnessObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
