using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSTFiling.Migrations
{
    /// <inheritdoc />
    public partial class SingleLoginTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the two link columns to Users
            migrationBuilder.AddColumn<int>(
                name: "ConsultantId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "Users",
                type: "int",
                nullable: true);

            // 2. Copy existing consultant and shop logins into Users,
            //    BEFORE their Password columns are removed
            migrationBuilder.Sql(@"
                INSERT INTO Users (UserName, Email, Password, RoleId, IsActive, CentreId, ConsultantId, UpdatedAt)
                SELECT c.Name, c.Email, LEFT(c.Password, 50), r.Id, c.IsActive, c.CentreId, c.Id, CONVERT(nvarchar(50), GETDATE(), 120)
                FROM Consultants c
                CROSS JOIN Roles r
                WHERE r.Name = 'Consultant'
                  AND c.Email IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM Users u WHERE u.Email = c.Email);");

            migrationBuilder.Sql(@"
                INSERT INTO Users (UserName, Email, Password, RoleId, IsActive, CentreId, ShopId, UpdatedAt)
                SELECT s.ShopName, s.Email, LEFT(s.Password, 50), r.Id, 1, s.CentreId, s.Id, CONVERT(nvarchar(50), GETDATE(), 120)
                FROM Customers s
                CROSS JOIN Roles r
                WHERE r.Name = 'Shop'
                  AND s.Email IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM Users u WHERE u.Email = s.Email);");

            // 3. Now remove the old Password columns
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Consultants");

            // 4. One email = one account
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ConsultantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Customers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Consultants",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
