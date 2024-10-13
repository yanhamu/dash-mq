using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DashMq.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnDirection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "direction",
                table: "datapoints",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "direction",
                table: "datapoints");
        }
    }
}
