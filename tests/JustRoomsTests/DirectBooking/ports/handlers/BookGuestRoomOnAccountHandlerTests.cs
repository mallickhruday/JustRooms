using System;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.adapters.data;
using DirectBooking.application;
using DirectBooking.ports.commands;
using DirectBooking.ports.events;
using DirectBooking.ports.handlers;
using FakeItEasy;
using JustSaying.Messaging;
using NUnit.Framework;

namespace JustRoomsTests.DirectBooking.ports.handlers
{
    [TestFixture]
    public class BookGuestRoomOnAccountHandlerTests
    {
        private InMemoryUnitOfWork _unitOfWork;

        [SetUp]
        public void Initialize()
        {
            _unitOfWork = new InMemoryUnitOfWork();
        }

        [Test]
        public async Task When_adding_a_room_booking_on_account()
        {
            //arrange
            var booking = new BookGuestRoomOnAccount()
            {
                BookingId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString(),
                DateOfFirstNight = DateTime.UtcNow,
                NumberOfNights = 1,
                NumberOfGuests = 1,
                Type = RoomType.King,
                Price = new Money(120, "GBP")
                
            };

            var messagePublisher = A.Fake<IMessagePublisher>();
            var handler = new BookGuestRoomOnAccountHandlerAsync(_unitOfWork, messagePublisher);

            //act
            await handler.HandleAsync(booking);

            var savedBooking = await _unitOfWork.GetAsync(Guid.Parse(booking.BookingId));
            Assert.That(savedBooking.BookingId, Is.EqualTo(booking.BookingId));
            Assert.That(savedBooking.DateOfFirstNight, Is.EqualTo(booking.DateOfFirstNight));
            Assert.That(savedBooking.NumberOfNights, Is.EqualTo(booking.NumberOfNights));
            Assert.That(savedBooking.NumberOfGuests, Is.EqualTo(booking.NumberOfGuests));
            Assert.That(savedBooking.RoomType, Is.EqualTo(booking.Type));
            Assert.That(savedBooking.Price, Is.EqualTo(booking.Price));
            Assert.That(savedBooking.AccountId, Is.EqualTo(booking.AccountId));

            A.CallTo(() => messagePublisher.PublishAsync(A<GuestRoomBookingMade>._, A<PublishMetadata>._, A<CancellationToken>._)).MustHaveHappened();
        }

    }
}