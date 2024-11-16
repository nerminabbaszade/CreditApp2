using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixCustomerApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomerApproved",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustomerApproved",
                table: "Loans");
        }
    }
}
