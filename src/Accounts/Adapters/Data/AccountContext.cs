using Accounts.Application;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Adapters.Data
{
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts {get; set; }

        protected AccountContext  () {}

        public AccountContext(DbContextOptions<AccountContext> options) : base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }

            // Fixes issue with MySql connector reporting nested transactions not supported https://github.com/aspnet/EntityFrameworkCore/issues/7017
            //Database.AutoTransactionsEnabled = false;

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(field => field.AccountId)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Account>()
                .HasKey(field => field.AccountId);
        }
    }
}