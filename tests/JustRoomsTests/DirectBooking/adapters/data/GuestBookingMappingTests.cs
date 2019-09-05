using System;
using System.Collections.Generic;
using DirectBooking.application;
using NUnit.Framework;

namespace Tests.DirectBooking.adapters.data
{
    [TestFixture]
    public class GuestBookingMappingTests
    {
        [Test]
        public void When_mapping_a_guest_room_booking()
        {
            //arrange
            
            
            var booking = new RoomBooking
            {
                BookingId = Guid.NewGuid(),
                DateOfFirstNight = new DateTime(2019, 07, 11),
                Price = new Money{Amount = 226, Currency = "USD"},
                Type = RoomType.MasterSuite, 
                NumberOfGuests = 3,
                NumberOfNights = 3,
                AccountId = Guid.NewGuid().ToString()
           };
            
            //act
            //assert
        }
    }
}