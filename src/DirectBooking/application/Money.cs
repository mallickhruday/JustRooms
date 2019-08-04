namespace DirectBooking.application
{
    public struct Money
    {
        public Money(int Amount, string Currency)
        {
            this.Amount = Amount;
            this.Currency = Currency;
        }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}