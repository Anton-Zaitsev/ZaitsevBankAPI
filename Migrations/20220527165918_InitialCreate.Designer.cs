﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZaitsevBankAPI;

#nullable disable

namespace ZaitsevBankAPI.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20220527165918_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ZaitsevBankAPI.Models.CardModel", b =>
                {
                    b.Property<Guid>("CardID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CVV")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CardOperator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ClosedCard")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DataClosedCard")
                        .HasColumnType("datetime2");

                    b.Property<double>("MoneyCard")
                        .HasColumnType("float");

                    b.Property<string>("NameCard")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumberCard")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TransactionCard")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TypeMoney")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CardID");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.Exchange", b =>
                {
                    b.Property<string>("IDValute")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("ChangesBuy")
                        .HasColumnType("bit");

                    b.Property<bool>("ChangesSale")
                        .HasColumnType("bit");

                    b.Property<string>("CharCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ElectronValute")
                        .HasColumnType("bit");

                    b.Property<string>("NameValute")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ValuteBuy")
                        .HasColumnType("float");

                    b.Property<double>("ValuteSale")
                        .HasColumnType("float");

                    b.HasKey("IDValute");

                    b.ToTable("Exchanges");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.Credits", b =>
                {
                    b.Property<Guid>("TransactionsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("CreditSumm")
                        .HasColumnType("float");

                    b.Property<int>("NumberDocument")
                        .HasColumnType("int");

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.Property<float>("Rate")
                        .HasColumnType("real");

                    b.Property<Guid>("TransactionCard")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TransactionsID");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.CurrencyTransfer", b =>
                {
                    b.Property<Guid>("TransactionsID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("ActualCurseRub")
                        .HasColumnType("float");

                    b.Property<bool>("BuySaleValute")
                        .HasColumnType("bit");

                    b.Property<int>("NumberDocument")
                        .HasColumnType("int");

                    b.Property<Guid>("TransactionCardFrom")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TransactionCardTo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ValuteA")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ValuteB")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TransactionsID");

                    b.ToTable("CurrencyTransfers");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.PaymentServices", b =>
                {
                    b.Property<Guid>("TransactionsID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CorrespondentScore")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CurrencyTransferID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DebitCard")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NameServices")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberDocument")
                        .HasColumnType("int");

                    b.Property<bool?>("Payment")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TransactionsID");

                    b.ToTable("PaymentServices");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.Transactions", b =>
                {
                    b.Property<Guid>("TransactionsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Arrival")
                        .HasColumnType("float");

                    b.Property<DateTime?>("ArrivalDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CodeOperation")
                        .HasColumnType("int");

                    b.Property<Guid?>("CreditsTransactionsID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Expenses")
                        .HasColumnType("float");

                    b.Property<DateTime?>("ExpensesDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NameOperation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TransferClientTransactionsID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ValuteTransactions")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TransactionsID");

                    b.HasIndex("CreditsTransactionsID");

                    b.HasIndex("TransferClientTransactionsID");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.TransferClient", b =>
                {
                    b.Property<Guid>("TransactionsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CurrencyTransferID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NameRecipient")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameSender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberDocument")
                        .HasColumnType("int");

                    b.Property<Guid>("TransactionRecipient")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TransactionSender")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserRecipient")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserSender")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TransactionsID");

                    b.ToTable("TransferClient");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.UserModel", b =>
                {
                    b.Property<Guid>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Birthday")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.CurrencyTransfer", b =>
                {
                    b.HasOne("ZaitsevBankAPI.Models.TransactionsModels.Transactions", "Transactions")
                        .WithOne("CurrencyTransfer")
                        .HasForeignKey("ZaitsevBankAPI.Models.TransactionsModels.CurrencyTransfer", "TransactionsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.PaymentServices", b =>
                {
                    b.HasOne("ZaitsevBankAPI.Models.TransactionsModels.Transactions", "Transactions")
                        .WithOne("PaymentServices")
                        .HasForeignKey("ZaitsevBankAPI.Models.TransactionsModels.PaymentServices", "TransactionsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.Transactions", b =>
                {
                    b.HasOne("ZaitsevBankAPI.Models.TransactionsModels.Credits", "Credits")
                        .WithMany("Transactions")
                        .HasForeignKey("CreditsTransactionsID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ZaitsevBankAPI.Models.TransactionsModels.TransferClient", "TransferClient")
                        .WithMany("Transactions")
                        .HasForeignKey("TransferClientTransactionsID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Credits");

                    b.Navigation("TransferClient");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.Credits", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.Transactions", b =>
                {
                    b.Navigation("CurrencyTransfer");

                    b.Navigation("PaymentServices");
                });

            modelBuilder.Entity("ZaitsevBankAPI.Models.TransactionsModels.TransferClient", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
