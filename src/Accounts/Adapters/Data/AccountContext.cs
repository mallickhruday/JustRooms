using Accounts.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Accounts.Adapters.Data
{
    /// <summary>
    /// A EF unit of work for Accounts
    /// </summary>
    public class AccountContext : DbContext
    {
        /// <summary>
        /// Our set of accounts
        /// </summary>
        public DbSet<Account> Accounts {get; set; }

        /// <summary>
        /// create am account context 
        /// </summary>
        protected AccountContext  () {}

        /// <summary>
        /// create an account context against a Db
        /// </summary>
        /// <param name="options"></param>
        public AccountContext(DbContextOptions<AccountContext> options) : base(options){}

        /// <summary>
        /// Configure our database
        /// </summary>
        /// <param name="optionsBuilder"></param>
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