using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrostAura.Clients.PointsKeeper.Data.Migrations.PointsKeeperDb
{
    /// <inheritdoc />
    public partial class AddBase64LogoToDonor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Base64Icon",
                table: "Donors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Base64Icon",
                table: "Donors");
        }
    }
}
