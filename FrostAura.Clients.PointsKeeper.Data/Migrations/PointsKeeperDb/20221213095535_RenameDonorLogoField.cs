using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrostAura.Clients.PointsKeeper.Data.Migrations.PointsKeeperDb
{
    /// <inheritdoc />
    public partial class RenameDonorLogoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Base64Icon",
                table: "Donors",
                newName: "LogoUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "Donors",
                newName: "Base64Icon");
        }
    }
}
