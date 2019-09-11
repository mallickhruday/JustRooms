using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Amazon.DynamoDBv2.Model;
using DirectBooking.adapters.data;
using DirectBooking.application;
using DirectBooking.ports.repositories;
using JustRoomsTests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Outbox.DynamoDB;
using EFUnitOfWork = DirectBooking.adapters.data.EFUnitOfWork;

namespace Tests.DirectBooking.adapters.data
{
    [TestFixture]
    public class RoomBookingMappingTests 
    {
        private DbContextOptions<BookingContext> _options;

        [SetUp]
        public void Initialise()
        {
            _options = new DbContextOptionsBuilder<BookingContext>()
                .UseMySql( "Server=localhost;Uid=root;Pwd=root;Database=Bookings")
                .Options;
            
            using (var context = new BookingContext(_options))
            {
                context.Database.EnsureCreated();
            }
 
        }
        
        [Test]
        public async Task When_mapping_a_guest_room_booking()
        {
            using (var uow = new BookingContext(_options))
            {
                //arrange
                var booking = new RoomBooking
                {
                    RoomBookingId = Guid.NewGuid(),
                    DateOfFirstNight = new DateTime(2019, 07, 11),
                    Price = 226,
                    RoomType = RoomType.MasterSuite,
                    NumberOfGuests = 3,
                    NumberOfNights = 3,
                    AccountId = Guid.NewGuid().ToString(),
                    LockedBy = "SYS",
                    LockExpiresAt = DateTime.Now.AddMilliseconds(500).Ticks.ToString()

                };

                var roomRepository = new RoomBookingRepositoryAsync(new EFUnitOfWork(uow));

                //act
                await roomRepository.AddAsync(booking);

                //assert
                await Task.Delay(50);

                var savedBooking = await roomRepository.GetAsync(booking.RoomBookingId);

                Assert.That(savedBooking.RoomBookingId, Is.EqualTo(booking.RoomBookingId));
                Assert.That(savedBooking.DateOfFirstNight, Is.EqualTo(booking.DateOfFirstNight));
                Assert.That(savedBooking.Price, Is.EqualTo(booking.Price));
                Assert.That(savedBooking.RoomType, Is.EqualTo(booking.RoomType));
                Assert.That(savedBooking.NumberOfGuests, Is.EqualTo(booking.NumberOfGuests));
                Assert.That(savedBooking.NumberOfNights, Is.EqualTo(booking.NumberOfNights));
                Assert.That(savedBooking.AccountId, Is.EqualTo(booking.AccountId));
            }
        }
   }
}