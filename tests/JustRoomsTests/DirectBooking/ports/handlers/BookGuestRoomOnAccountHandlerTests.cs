using System;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.adapters.data;
using DirectBooking.application;
using DirectBooking.ports.commands;
using DirectBooking.ports.events;
using DirectBooking.ports.handlers;
using DirectBooking.ports.repositories;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Paramore.Brighter;

namespace JustRoomsTests.DirectBooking.ports.handlers
{
    [TestFixture]
    public class BookGuestRoomOnAccountHandlerTests
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
        public async Task When_adding_a_room_booking_on_account()
        {
            using (var uow = new BookingContext(_options))
            {
                //arrange
                var booking = new BookGuestRoomOnAccount()
                {
                    BookingId = Guid.NewGuid(),
                    AccountId = Guid.NewGuid().ToString(),
                    DateOfFirstNight = DateTime.UtcNow,
                    NumberOfNights = 1,
                    NumberOfGuests = 1,
                    Type = RoomType.King,
                    Price = 120
                };
                
                var commandProcessor = new FakeCommandProcessor();
                var handler = new BookGuestRoomOnAccountHandlerAsync(_options, commandProcessor);

                //act
                await handler.HandleAsync(booking);

                //assert
                var roomBookingRepositoryAsync = new RoomBookingRepositoryAsync(new EFUnitOfWork(uow));
                var savedBooking = await roomBookingRepositoryAsync.GetAsync(booking.BookingId);
                
                Assert.That(savedBooking.RoomBookingId, Is.EqualTo(booking.BookingId));
                Assert.That(savedBooking.DateOfFirstNight, Is.EqualTo(booking.DateOfFirstNight));
                Assert.That(savedBooking.NumberOfNights, Is.EqualTo(booking.NumberOfNights));
                Assert.That(savedBooking.NumberOfGuests, Is.EqualTo(booking.NumberOfGuests));
                Assert.That(savedBooking.RoomType, Is.EqualTo(booking.Type));
                Assert.That(savedBooking.Price, Is.EqualTo(booking.Price));
                Assert.That(savedBooking.AccountId, Is.EqualTo(booking.AccountId));
                
                Assert.That(commandProcessor.AllSent);
                Assert.That(commandProcessor.RaiseRoomBookingEvent);
            }
        }

    }
}