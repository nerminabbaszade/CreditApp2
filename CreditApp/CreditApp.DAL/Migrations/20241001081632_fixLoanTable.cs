using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixLoanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Loans",
                newName: "IsAccepted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAccepted",
                table: "Loans",
                newName: "IsActive");
        }
    }
}
