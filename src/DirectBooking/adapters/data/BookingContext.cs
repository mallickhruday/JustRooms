using DirectBooking.application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DirectBooking.adapters.data
{
    public class BookingContext : DbContext
    {
        public DbSet<RoomBooking> Bookings {get; set; }

        protected BookingContext () {}

        public BookingContext (DbContextOptions<BookingContext> options) : base(options){}

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
            modelBuilder.Entity<RoomBooking>()
                .Property(field => field.RoomBookingId)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<RoomBooking>()
                .HasKey(field => field.RoomBookingId);
        }
    }
}