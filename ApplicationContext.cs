using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.Models;
using ZaitsevBankAPI.Models.TransactionsModels;

namespace ZaitsevBankAPI
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CardModel> Cards { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Credits> Credits { get; set; }
        public DbSet<CurrencyTransfer> CurrencyTransfers { get; set; }
        public DbSet<TransferClient> TransferClient { get; set; }
        public DbSet<PaymentServices> PaymentServices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=wpl36.hosting.reg.ru;Database=u1643975_zaitsevBank;User ID=u1643975_anton;Password=Antonio19z01a01d;", builder =>
             {
                 builder.EnableRetryOnFailure(5,TimeSpan.FromSeconds(10),null);
             });
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentServices>()
                .HasOne(a => a.Transactions)
                .WithOne(b => b.PaymentServices)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CurrencyTransfer>()
                .HasOne(a => a.Transactions)
                .WithOne(b => b.CurrencyTransfer)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransferClient>()
                .HasMany(a => a.Transactions)
                .WithOne(b => b.TransferClient)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Credits>()
                .HasMany(a => a.Transactions)
                .WithOne(b => b.Credits)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
