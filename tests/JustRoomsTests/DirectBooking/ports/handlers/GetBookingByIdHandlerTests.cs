using System;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using DirectBooking.adapters.data;
using DirectBooking.application;
using DirectBooking.ports.handlers;
using DirectBooking.ports.queries;
using DirectBooking.ports.repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using EFUnitOfWork = DirectBooking.adapters.data.EFUnitOfWork;

namespace JustRoomsTests.DirectBooking.ports.handlers
{
    [TestFixture]
    public class GetBookingByIdHandlerTests
    {
        private DbContextOptions<BookingContext> _options;

        [SetUp]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<BookingContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
        }

        [Test]
        public async Task When_getting_a_booking_by_id()
        {
            using (var uow = new BookingContext(_options))
            {
                //arrange
                var booking = new RoomBooking
                {
                    RoomBookingId = Guid.NewGuid(),
                    DateOfFirstNight = new DateTime(2019, 07, 11),
                    Price = new Money(226, "USD"),
                    RoomType = RoomType.MasterSuite,
                    NumberOfGuests = 3,
                    NumberOfNights = 3,
                    AccountId = Guid.NewGuid().ToString(),
                    LockedBy = "SYS",
                    LockExpiresAt = DateTime.Now.AddMilliseconds(500).Ticks.ToString()

                };

                var repository = new RoomBookingRepositoryAsync(new EFUnitOfWork(uow));
                await repository.AddAsync(booking);

                var handler = new GetBookingByIdHandlerAsync(_options);

                //act
                var bookingResult = await handler.ExecuteAsync(new GetBookingById(booking.RoomBookingId));

                //assert
                Assert.That(bookingResult.BookingId, Is.EqualTo(booking.RoomBookingId));
                Assert.That(bookingResult.DateOfFirstNight, Is.EqualTo(booking.DateOfFirstNight));
                Assert.That(bookingResult.Price, Is.EqualTo(booking.Price));
                Assert.That(bookingResult.RoomType, Is.EqualTo(booking.RoomType));
                Assert.That(bookingResult.NumberOfNights, Is.EqualTo(booking.NumberOfNights));
                Assert.That(bookingResult.NumberOfGuests, Is.EqualTo(booking.NumberOfGuests));
                Assert.That(bookingResult.AccountId, Is.EqualTo(booking.AccountId));
            }
        }
    }
}