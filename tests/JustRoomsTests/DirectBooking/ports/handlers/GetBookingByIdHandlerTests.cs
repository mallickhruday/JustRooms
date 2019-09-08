using System;
using System.Threading.Tasks;
using DirectBooking.adapters.data;
using DirectBooking.application;
using DirectBooking.ports.handlers;
using DirectBooking.ports.queries;
using DirectBooking.ports.repositories;
using NUnit.Framework;

namespace JustRoomsTests.DirectBooking.ports.handlers
{
    [TestFixture]
    public class GetBookingByIdHandlerTests
    {
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Initialize()
        {
            _unitOfWork = new InMemoryUnitOfWork();
        }

        [Test]
        public async Task When_getting_a_booking_by_id()
        {
            //arrange
            var booking = new RoomBooking
            {
                BookingId = Guid.NewGuid().ToString(),
                DateOfFirstNight = new DateTime(2019, 07, 11),
                Price = new Money(226, "USD"),
                RoomType = RoomType.MasterSuite, 
                NumberOfGuests = 3,
                NumberOfNights = 3,
                AccountId = Guid.NewGuid().ToString(),
                CurrentVersion = 1,
                Version = RoomBooking.SnapShot,
                LockedBy = "SYS",
                LockExpiresAt = DateTime.Now.AddMilliseconds(500).Ticks.ToString()
 
           };

            await _unitOfWork.SaveAsync(booking);

            var handler = new GetBookingByIdHandlerAsync(_unitOfWork);
            
            //act
            var bookingResult = await handler.ExecuteAsync(new GetBookingById(Guid.Parse(booking.BookingId)));

            //assert
            Assert.That(bookingResult.BookingId, Is.EqualTo(booking.BookingId));
            Assert.That(bookingResult.DateOfFirstNight, Is.EqualTo(booking.DateOfFirstNight));
            Assert.That(bookingResult.Price, Is.EqualTo(booking.Price));
            Assert.That(bookingResult.RoomType, Is.EqualTo(booking.RoomType));
            Assert.That(bookingResult.NumberOfNights, Is.EqualTo(booking.NumberOfNights));
            Assert.That(bookingResult.NumberOfGuests, Is.EqualTo(booking.NumberOfGuests));
            Assert.That(bookingResult.AccountId, Is.EqualTo(booking.AccountId));
        }
    }
}