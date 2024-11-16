using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LoanTableasBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAccepted",
                table: "Loans",
                newName: "IsApproved");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "Loans",
                newName: "IsAccepted");
        }
    }
}
