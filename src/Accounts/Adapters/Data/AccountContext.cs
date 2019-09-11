using Accounts.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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

            // Fixes issue with MySql connector reporting InMemory Transactions not supported 
            optionsBuilder.ConfigureWarnings((warnings) => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
 
            base.OnConfiguring(optionsBuilder);
        }
   }
}