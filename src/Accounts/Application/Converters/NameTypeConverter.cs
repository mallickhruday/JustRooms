using System;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json.Linq;

namespace Accounts.Application.Converters
{
    public class NameTypeConverter : IPropertyConverter
    {
        private const string LastName = "lastName";
        private const string FirstName = "firstName";

        public DynamoDBEntry ToEntry(object value)
        {
            var name = value as Name;
            if (name == null) throw new InvalidOperationException($"Supplied type was of type {value.GetType().Name} not Accounts.Application.Name");

            var json = new JObject(new JProperty(FirstName, name.FirstName), new JProperty(LastName, name.LastName));
            
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
            
            var name = new Name((string)value[FirstName], (string)value[LastName]);
            return name;
        }
    }
}