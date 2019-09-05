using System;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json.Linq;

namespace Accounts.Application.Converters
{
    /// <summary>
    /// Convert card details composite type into a DynameDb type
    /// </summary>
    public class CardDetailsTypeConverter : IPropertyConverter
    {
        private const string CardNumber = "cardNumber";
        private const string CardSecurityCode = "cardSecurityCode";

        /// <summary>
        /// Convert card details into a DynamoDb entry
        /// </summary>
        /// <param name="value">The CardDetails object to convert</param>
        /// <returns>A DynamoDb string property</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DynamoDBEntry ToEntry(object value)
        {
            var cardDetails = value as CardDetails;
            if (cardDetails == null)
                throw new InvalidOperationException(
                    $"Supplied type was of type {value.GetType().Name} not Accounts.Application.CardDetails");

            var json = new JObject(new JProperty(CardNumber, cardDetails.CardNumber),
                new JProperty(CardSecurityCode, cardDetails.CardSecurityCode));

            DynamoDBEntry entry = new Primitive
            {
                Type = DynamoDBEntryType.String,
                Value = json.ToString()
            };

            return entry;
        }

        /// <summary>
        /// Convert from a DynamoDb entry into a CardDetails objecgt
        /// </summary>
        /// <param name="entry">A DynamoDb entry</param>
        /// <returns>A CardDetails object</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public object FromEntry(DynamoDBEntry entry)
        {
            var primitive = entry as Primitive;
            if (primitive == null || !(primitive.Value is String) || string.IsNullOrEmpty((string) primitive.Value))
                throw new ArgumentOutOfRangeException();

            var value = JObject.Parse(entry.AsString());

            var name = new CardDetails((string) value[CardNumber], (string) value[CardSecurityCode]);
            return name;
        }
    }
}