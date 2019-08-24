using System;
using System.Collections.Generic;
using AccountsTransferWorker.Application;
using AccountsTransferWorker.Ports;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace AccountsTransferWorker.Adapters.Data
{
    public class AccountFromRecordTranslator : IRecordTranslator<StreamRecord, AccountEvent>
    {
        public AccountEvent TranslateFromRecord(StreamRecord record)
        {
            var accountEvent = new AccountEvent();
            accountEvent.AccountId = record.NewImage["AccountId"].S;
            accountEvent.Name = JsonConvert.DeserializeObject<Name>(record.NewImage["Name"].S);
            var addresses = new List<Address>();
            foreach (var attributeValue in record.NewImage["Addresses"].L)
            {
                var address = JsonConvert.DeserializeObject<Address>(attributeValue.S);
                addresses.Add(address);
            }

            accountEvent.Addresses = addresses;
            accountEvent.ContactDetails = JsonConvert.DeserializeObject<ContactDetails>(record.NewImage["ContactDetails"].S);
            accountEvent.CardDetails = JsonConvert.DeserializeObject<CardDetails>(record.NewImage["CardDetails"].S);
            accountEvent.Version = Convert.ToInt32(record.NewImage["CurrentVersion"].S);
            return accountEvent;
        }
    }
}