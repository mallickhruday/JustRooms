using System;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json.Linq;

namespace DirectBooking.application.converters
{
    public class MoneyTypeConverter : IPropertyConverter
    {        
        private const string Amount = "amount";
        private const string Currency = "currency";

        public DynamoDBEntry ToEntry(object value)
        {
            var money = value as Money;
            if (money == null) throw new InvalidOperationException($"Supplied type was of type {value.GetType().Name} not DirectBooking.Application.Money");

            var json = new JObject(new JProperty(Amount, money.Amount), new JProperty(Currency, money.Currency));
            
            DynamoDBEntry entry = new Primitive
            {
                Type = DynamoDBEntryType.String,
                Value = json.ToString()
            };

            return entry;

        }

        public object FromEntry(DynamoDBEntry entry)
        {
            var primitive = entry as Primitive;
            if (primitive == null || !(primitive.Value is String) || string.IsNullOrEmpty((string)primitive.Value))
                throw new ArgumentOutOfRangeException();
            
            var value = JObject.Parse(entry.AsString());
            
            var name = new Money((int)value[Amount], (string)value[Currency]);
            return name;

        }
    }
}