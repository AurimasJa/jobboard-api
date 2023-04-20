using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jobboard.Migrations
{
    /// <inheritdoc />
    public partial class newupdatedsalaru : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SalaryUp",
                table: "Jobs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryUp",
                table: "Jobs");
        }
    }
}
