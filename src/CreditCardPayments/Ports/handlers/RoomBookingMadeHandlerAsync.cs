using System;
using System.Threading.Tasks;
using JustRooms.DirectBookingEventConsumer.Ports.events;
using JustSaying.Messaging.MessageHandling;

namespace JustRooms.DirectBookingEventConsumer.Ports.handlers
{
    public class RoomBookingMadeHandlerAsync : IHandlerAsync<GuestRoomBookingMade>
    {
        public Task<bool> Handle(GuestRoomBookingMade booking)
        {
            var tcs = new TaskCompletionSource<bool>();
            //TODO: Price does not seem to serialize correctly, maybe nested failing out of the box?
            Console.WriteLine($"Booking {booking.BookingId} for {booking.NumberOfNights} nights at {booking.Price.Currency}{booking.Price.Amount}" 
                              + $"from {booking.DateOfFirstNight} made by {booking.FirstName} {booking.LastName} for {booking.NumberOfGuests} guests.");
            tcs.SetResult(true);
            return tcs.Task;
        }
    }
}