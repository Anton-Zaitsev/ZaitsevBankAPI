using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZaitsevBankAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardOperator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosedCard = table.Column<bool>(type: "bit", nullable: false),
                    CVV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataClosedCard = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoneyCard = table.Column<double>(type: "float", nullable: false),
                    NameCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeMoney = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionCard = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
