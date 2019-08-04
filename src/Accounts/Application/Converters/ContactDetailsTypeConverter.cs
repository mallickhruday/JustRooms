using System;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json.Linq;

namespace Accounts.Application.Converters
{
    public class ContactDetailsTypeConverter : IPropertyConverter
    {
        private const string TelephoneNumber = "telephoneNumber";
        private const string Email = "email";

        public DynamoDBEntry ToEntry(object value)
        {
            var name = value as ContactDetails;
            if (name == null) throw new InvalidOperationException($"Supplied type was of type {value.GetType().Name} not Accounts.Application.ContactDetails");

            var json = new JObject(new JProperty(Email, name.Email), new JProperty(TelephoneNumber, name.TelephoneNumber));
            
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
            
            var name = new ContactDetails((string)value[Email], (string)value[TelephoneNumber]);
            return name;
        }
    }
}