using CreditCardCore.Application;
using CreditCardCore.Ports.Events;
using Microsoft.EntityFrameworkCore;

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
     
                 // Fixes issue with MySql connector reporting nested transactions not supported https://github.com/aspnet/EntityFrameworkCore/issues/7017
                 //Database.AutoTransactionsEnabled = false;
     
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