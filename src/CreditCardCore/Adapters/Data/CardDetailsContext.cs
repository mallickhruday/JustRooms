using CreditCardCore.Application;
using CreditCardCore.Ports.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CreditCardCore.Adapters.Data
{
    public class CardDetailsContext : DbContext
    {
             public DbSet<AccountCardDetails> Accounts {get; set; }
     
             protected CardDetailsContext   () {}
     
             public CardDetailsContext(DbContextOptions<CardDetailsContext> options) : base(options){}
     
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
     
             protected override void OnModelCreating(ModelBuilder modelBuilder)
             {
                 modelBuilder.Entity<AccountCardDetails>()
                     .Property(field => field.AccountId)
                     .ValueGeneratedOnAdd();
                 modelBuilder.Entity<AccountCardDetails>()
                     .HasKey(field => field.AccountId);
             }   
    }
}