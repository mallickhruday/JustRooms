using System.Threading;
using System.Threading.Tasks;
using DirectBooking.ports.commands;
using Paramore.Brighter;

namespace DirectBooking.ports.handlers
{
    public class BookGuestRoomHandler : RequestHandlerAsync<BookGuestRoom>
    {
        public override Task<BookGuestRoom> HandleAsync(BookGuestRoom command, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO: Add to Dynamo
            return base.HandleAsync(command, cancellationToken);
        }
    }
}