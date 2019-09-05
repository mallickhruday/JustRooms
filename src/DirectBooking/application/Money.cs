using System;

namespace DirectBooking.application
{
    public class Money : IEquatable<Money>
    {
       public Money(int Amount, string Currency)
        {
            this.Amount = Amount;
            this.Currency = Currency;
        }
        public double Amount { get; set; }
        public string Currency { get; set; }
        
        public bool Equals(Money other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Amount.Equals(other.Amount) && Currency == other.Currency;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Money) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Amount.GetHashCode() * 397) ^ (Currency != null ? Currency.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Money left, Money right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Money left, Money right)
        {
            return !Equals(left, right);
        }

     }
}