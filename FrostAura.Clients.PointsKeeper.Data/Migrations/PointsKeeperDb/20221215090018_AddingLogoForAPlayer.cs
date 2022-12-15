using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrostAura.Clients.PointsKeeper.Data.Migrations.PointsKeeperDb
{
    /// <inheritdoc />
    public partial class AddingLogoForAPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Players");
        }
    }
}
