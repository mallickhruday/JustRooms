using System.Threading.Tasks;
using DirectBooking.ports.commands;
using DirectBooking.ports.repositories;

namespace DirectBooking.adapters.data
{
    public class GuestBookingRepository : IGuestBookingRepository
    {
        public async Task Add(BookGuestRoom booking)
        {
            throw new System.NotImplementedException();
        }
    }
}