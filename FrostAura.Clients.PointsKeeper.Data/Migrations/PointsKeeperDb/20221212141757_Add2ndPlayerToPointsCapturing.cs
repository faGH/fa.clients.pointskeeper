using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrostAura.Clients.PointsKeeper.Data.Migrations.PointsKeeperDb
{
    /// <inheritdoc />
    public partial class Add2ndPlayerToPointsCapturing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Points_Players_PlayerId",
                table: "Points");

            migrationBuilder.DropIndex(
                name: "IX_Points_PlayerId",
                table: "Points");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "Points",
                newName: "Player2Score");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Points",
                newName: "Player2Id");

            migrationBuilder.AddColumn<int>(
                name: "Player1Id",
                table: "Points",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player1Score",
                table: "Points",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Points_Player1Id",
                table: "Points",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Points_Player2Id",
                table: "Points",
                column: "Player2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Points_Players_Player1Id",
                table: "Points",
                column: "Player1Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Points_Players_Player2Id",
                table: "Points",
                column: "Player2Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Points_Players_Player1Id",
                table: "Points");

            migrationBuilder.DropForeignKey(
                name: "FK_Points_Players_Player2Id",
                table: "Points");

            migrationBuilder.DropIndex(
                name: "IX_Points_Player1Id",
                table: "Points");

            migrationBuilder.DropIndex(
                name: "IX_Points_Player2Id",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "Player1Id",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "Player1Score",
                table: "Points");

            migrationBuilder.RenameColumn(
                name: "Player2Score",
                table: "Points",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "Player2Id",
                table: "Points",
                newName: "Count");

            migrationBuilder.CreateIndex(
                name: "IX_Points_PlayerId",
                table: "Points",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Points_Players_PlayerId",
                table: "Points",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
