using System.Threading.Tasks;
using DirectBooking.ports.commands;

namespace DirectBooking.ports.repositories
{
    public interface IGuestBookingRepository
    {
        Task Add(BookGuestRoom booking);
    }
}