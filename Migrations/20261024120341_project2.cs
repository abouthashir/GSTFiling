using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSTFiling.Migrations
{
    /// <inheritdoc />
    public partial class project2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_ctype",
                columns: table => new
                {
                    ctypid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ctype = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_ctype", x => x.ctypid);
                });

            migrationBuilder.CreateTable(
                name: "tbl_month",
                columns: table => new
                {
                    mid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    month = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_month", x => x.mid);
                });

            migrationBuilder.CreateTable(
                name: "tbl_year",
                columns: table => new
                {
                    yid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_year", x => x.yid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_ctype");

            migrationBuilder.DropTable(
                name: "tbl_month");

            migrationBuilder.DropTable(
                name: "tbl_year");
        }
    }
}
