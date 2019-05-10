namespace JustRooms.GuestDirectBooking.ports.events
{
    public struct Money
    {
        public Money(int Amount, string Currency)
        {
            this.Amount = Amount;
            this.Currency = Currency;
        }
        public double Amount { get;}
        public string Currency { get;}
    }
}