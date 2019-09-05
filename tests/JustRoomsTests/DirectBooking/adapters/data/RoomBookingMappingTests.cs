using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using DirectBooking.adapters.data;
using DirectBooking.application;
using JustRoomsTests;
using NUnit.Framework;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Outbox.DynamoDB;

namespace Tests.DirectBooking.adapters.data
{
    [TestFixture]
    public class RoomBookingMappingTests : DynamoDbBaseTest
    {
        [Test]
        public async Task When_mapping_a_guest_room_booking()
        {
            //arrange
            var booking = new RoomBooking
            {
                BookingId = Guid.NewGuid().ToString(),
                DateOfFirstNight = new DateTime(2019, 07, 11),
                Price = new Money(226, "USD"),
                Type = RoomType.MasterSuite, 
                NumberOfGuests = 3,
                NumberOfNights = 3,
                AccountId = Guid.NewGuid().ToString(),
                CurrentVersion = 1,
                Version = RoomBooking.SnapShot,
                LockedBy = "SYS",
                LockExpiresAt = DateTime.Now.AddMilliseconds(500).Ticks.ToString()
 
           };

            var roomRepository = new DynamoDbUnitOfWork(Client);

            //act
            await roomRepository.SaveAsync(booking);
            
            //assert

            await Task.Delay(50);
            
            var savedBooking = await roomRepository.GetAsync(Guid.Parse(booking.BookingId));
            
            Assert.That(savedBooking.BookingId, Is.EqualTo(booking.BookingId));
            Assert.That(savedBooking.DateOfFirstNight, Is.EqualTo(booking.DateOfFirstNight));
            Assert.That(savedBooking.Price, Is.EqualTo(booking.Price));
            Assert.That(savedBooking.Type, Is.EqualTo(booking.Type));
            Assert.That(savedBooking.NumberOfGuests, Is.EqualTo(booking.NumberOfGuests));
            Assert.That(savedBooking.NumberOfNights, Is.EqualTo(booking.NumberOfNights));
            Assert.That(savedBooking.AccountId, Is.EqualTo(booking.AccountId));
        }

        protected override CreateTableRequest CreateTableRequest()
        {
            var createTableRequest = new DynamoDbTableFactory().GenerateCreateTableMapper<RoomBooking>(
                new DynamoDbCreateProvisionedThroughput(
                    new ProvisionedThroughput {ReadCapacityUnits = 10, WriteCapacityUnits = 10},
                    new Dictionary<string, ProvisionedThroughput>()
                ));
            return createTableRequest;
         }
    }
}