using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json.Linq;

namespace Accounts.Application.Converters
{
    public class AddressListTypeConverter : IPropertyConverter
    {
        private const string FirstLineOfAddress = "firstLineOfAddress";
        private const string AddressType = "addressType";
        private const string State = "state";
        private const string Zipcode = "zipCode";

        public DynamoDBEntry ToEntry(object value)
        {
            var addresses = value as List<Address>;
            if (addresses == null) throw new InvalidOperationException($"Supplied type was of type {value.GetType().Name} not List<Accounts.Application.Address>");


            var list = new DynamoDBList();
            foreach (var address in addresses)
            {
                var json = new JObject(
                    new JProperty(FirstLineOfAddress, address.FistLineOfAddress), 
                    new JProperty(AddressType, address.AddressType.ToString()), 
                    new JProperty(State, address.State),
                    new JProperty(Zipcode, address.ZipCode)
                );

                list.Add(new Primitive
                {
                    Type = DynamoDBEntryType.String,
                    Value = json.ToString()
                });
                
            }

            return list;
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            var list = entry as DynamoDBList;
            if (list == null)
                throw new ArgumentOutOfRangeException();

            var addresses = new List<Address>();
            foreach (var dbEntry in list.Entries)
            {
                var value = JObject.Parse(dbEntry.AsString());
            
                var address = new Address(
                    (string)value[FirstLineOfAddress],
                    Enum.Parse<AddressType>((string)value[AddressType]),
                    (string)value[State],
                    (string)value[Zipcode]
                );
             
                addresses.Add(address);
            }

            return addresses;
        }
    }
}