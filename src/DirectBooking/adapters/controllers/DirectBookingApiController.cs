using System;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.adapters.dtos;
using DirectBooking.application;
using DirectBooking.ports.commands;
using DirectBooking.ports.queries;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace DirectBooking.adapters.controllers
{
    [ApiController]
    public class DirectBookingApiController : ControllerBase
    {
         private readonly IAmACommandProcessor _commandProcessor;
         private readonly IQueryProcessor _queryProcessor;

         public DirectBookingApiController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
         {
             _commandProcessor = commandProcessor;
             _queryProcessor = queryProcessor;
         }
     
        [HttpGet("/booking/{id}", Name = "Get_Booking")]
        public async Task<IActionResult> Get(string id, CancellationToken ct)
        {
            var bookingById = new GetBookingById(Guid.Parse(id));
            var booking = await _queryProcessor.ExecuteAsync(bookingById, ct); 
            return Ok(RoomBookingDTO.FromQueryResult(booking));
        }

        [HttpPost("/bookings", Name = "Add_Booking")]
        public async Task<IActionResult> Post([FromBody] RoomBookingDTO roomBookingDto, CancellationToken ct)
        {
            var addBooking = new BookGuestRoomOnAccount(
                Guid.NewGuid(),
                roomBookingDto.DateOfFirstNight,
                Enum.Parse<RoomType>(roomBookingDto.RoomType),
                Convert.ToDouble(roomBookingDto.Amount),
                roomBookingDto.NumberOfNights,
                roomBookingDto.NumberOfGuests,
                roomBookingDto.AccountId
            );
            
            await _commandProcessor.SendAsync(addBooking, false, ct);
            var booking = await _queryProcessor.ExecuteAsync(new GetBookingById(addBooking.BookingId), ct); 
            return Ok(RoomBookingDTO.FromQueryResult(booking));
        }
    }
}