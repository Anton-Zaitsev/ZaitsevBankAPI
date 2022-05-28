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
                name: "Credits",
                columns: table => new
                {
                    TransactionsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberDocument = table.Column<int>(type: "int", nullable: false),
                    TransactionCard = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditSumm = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<float>(type: "real", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.TransactionsID);
                });

            migrationBuilder.CreateTable(
                name: "Exchanges",
                columns: table => new
                {
                    IDValute = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameValute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangesBuy = table.Column<bool>(type: "bit", nullable: false),
                    ValuteBuy = table.Column<double>(type: "float", nullable: false),
                    ChangesSale = table.Column<bool>(type: "bit", nullable: false),
                    ValuteSale = table.Column<double>(type: "float", nullable: false),
                    ElectronValute = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.IDValute);
                });

            migrationBuilder.CreateTable(
                name: "TransferClient",
                columns: table => new
                {
                    TransactionsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberDocument = table.Column<int>(type: "int", nullable: false),
                    TransactionSender = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionRecipient = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameSender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameRecipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserSender = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserRecipient = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyTransferID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferClient", x => x.TransactionsID);
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

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodeOperation = table.Column<int>(type: "int", nullable: false),
                    NameOperation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpensesDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Arrival = table.Column<double>(type: "float", nullable: true),
                    Expenses = table.Column<double>(type: "float", nullable: true),
                    ValuteTransactions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditsTransactionsID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransferClientTransactionsID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionsID);
                    table.ForeignKey(
                        name: "FK_Transactions_Credits_CreditsTransactionsID",
                        column: x => x.CreditsTransactionsID,
                        principalTable: "Credits",
                        principalColumn: "TransactionsID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_TransferClient_TransferClientTransactionsID",
                        column: x => x.TransferClientTransactionsID,
                        principalTable: "TransferClient",
                        principalColumn: "TransactionsID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyTransfers",
                columns: table => new
                {
                    TransactionsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberDocument = table.Column<int>(type: "int", nullable: false),
                    TransactionCardFrom = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionCardTo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValuteA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValuteB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuySaleValute = table.Column<bool>(type: "bit", nullable: false),
                    ActualCurseRub = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTransfers", x => x.TransactionsID);
                    table.ForeignKey(
                        name: "FK_CurrencyTransfers_Transactions_TransactionsID",
                        column: x => x.TransactionsID,
                        principalTable: "Transactions",
                        principalColumn: "TransactionsID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentServices",
                columns: table => new
                {
                    TransactionsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberDocument = table.Column<int>(type: "int", nullable: false),
                    DebitCard = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameServices = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyTransferID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CorrespondentScore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payment = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentServices", x => x.TransactionsID);
                    table.ForeignKey(
                        name: "FK_PaymentServices_Transactions_TransactionsID",
                        column: x => x.TransactionsID,
                        principalTable: "Transactions",
                        principalColumn: "TransactionsID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreditsTransactionsID",
                table: "Transactions",
                column: "CreditsTransactionsID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransferClientTransactionsID",
                table: "Transactions",
                column: "TransferClientTransactionsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "CurrencyTransfers");

            migrationBuilder.DropTable(
                name: "Exchanges");

            migrationBuilder.DropTable(
                name: "PaymentServices");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Credits");

            migrationBuilder.DropTable(
                name: "TransferClient");
        }
    }
}
