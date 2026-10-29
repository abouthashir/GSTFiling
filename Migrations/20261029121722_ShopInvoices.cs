using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSTFiling.Migrations
{
    /// <inheritdoc />
    public partial class ShopInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    invid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cstmrid = table.Column<int>(type: "int", nullable: false),
                    invno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    invdate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    taxable_amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    gst_amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    invyear = table.Column<int>(type: "int", nullable: false),
                    invmonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    taxrate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sgst = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    cgst = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    cess = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    billamtstr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.invid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoice");
        }
    }
}
