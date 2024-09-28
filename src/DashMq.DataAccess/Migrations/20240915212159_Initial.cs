using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DashMq.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "datapoints",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    topic = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_datapoints", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "datapoint_values",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    datapoint_id = table.Column<int>(type: "INTEGER", nullable: false),
                    timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    value = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_datapoint_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_datapoint_values_datapoints_datapoint_id",
                        column: x => x.datapoint_id,
                        principalTable: "datapoints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_datapoint_values_datapoint_id",
                table: "datapoint_values",
                column: "datapoint_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "datapoint_values");

            migrationBuilder.DropTable(
                name: "datapoints");
        }
    }
}
