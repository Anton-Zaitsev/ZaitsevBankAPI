using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.Models;

namespace ZaitsevBankAPI
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CardModel> Cards { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=wpl36.hosting.reg.ru;Database=u1643975_zaitsevBank;User ID=u1643975_anton;Password=Antonio19z01a01d;", builder =>
             {
                 builder.EnableRetryOnFailure(5,TimeSpan.FromSeconds(10),null);
             });
        }
    }
}
