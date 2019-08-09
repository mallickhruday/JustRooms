using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Amazon.DynamoDBv2.Model;
using NUnit.Framework;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Outbox.DynamoDB;

namespace JustRoomsTests.Accounts.adapters.data
{
    [TestFixture]
    public class AccountMappingTests : DynamoDbBaseTest
    { 
        [Test]
        public async Task When_adding_an_account()
        {
            //arrange
            var id = Guid.NewGuid();
            var account = new Account()
            {
                AccountId = id.ToString(),
                Name = new Name("Jack", "Torrance"),
                Addresses = new List<Address>
                {
                    new Address("Overlook Hotel", AddressType.Work, "CO", "80517"),
                    new Address("Overlook Hotel",AddressType.Billing,"CO", "80517")
                },
                ContactDetails = new ContactDetails("jack.torrance@shining.com", "666-6666"),
                CardDetails  = new CardDetails("4104231121998973", "517"),
                CurrentVersion = 1,
                Version = Account.SnapShot,
                LockedBy = "SYS",
                LockExpiresAt = DateTime.Now.AddMilliseconds(500).Ticks.ToString()
            };

            var accountRepository = new UnitOfWork(Client);

            //act
            await accountRepository.SaveAsync(account);

            await Task.Delay(50);

            var savedAccount = await accountRepository.GetAsync(id);

            //assert
            Assert.That(savedAccount.AccountId, Is.EqualTo(id.ToString()));
            Assert.That(savedAccount.Name.FirstName, Is.EqualTo(account.Name.FirstName));
            Assert.That(savedAccount.Name.LastName, Is.EqualTo(account.Name.LastName));
            Assert.That(savedAccount.Addresses.First().AddressType, Is.EqualTo(account.Addresses.First().AddressType));
            Assert.That(savedAccount.Addresses.First().FistLineOfAddress, Is.EqualTo(account.Addresses.First().FistLineOfAddress));
            Assert.That(savedAccount.Addresses.First().State, Is.EqualTo(account.Addresses.First().State));
            Assert.That(savedAccount.Addresses.First().ZipCode, Is.EqualTo(account.Addresses.First().ZipCode));
            Assert.That(savedAccount.ContactDetails.Email, Is.EqualTo(savedAccount.ContactDetails.Email));
            Assert.That(savedAccount.CardDetails.CardSecurityCode, Is.EqualTo(savedAccount.CardDetails.CardSecurityCode));
            Assert.That(savedAccount.ContactDetails.Email, Is.EqualTo(savedAccount.ContactDetails.Email));
            Assert.That(savedAccount.ContactDetails.TelephoneNumber, Is.EqualTo(account.ContactDetails.TelephoneNumber));
            Assert.That(savedAccount.CurrentVersion, Is.EqualTo(account.CurrentVersion));
            Assert.That(savedAccount.Version, Is.EqualTo(account.Version));
            Assert.That(savedAccount.LockedBy, Is.EqualTo(account.LockedBy));
            Assert.That(savedAccount.LockExpiresAt, Is.EqualTo(account.LockExpiresAt));
         }

        [Test]
        public async Task When_deleting_an_account()
        {
            var id = Guid.NewGuid();
            var account = new Account()
            {
                AccountId = id.ToString(),
                Name = new Name("Jack", "Torrance"),
                Addresses = new List<Address>
                {
                    new Address("Overlook Hotel", AddressType.Work, "CO", "80517"),
                    new Address("Overlook Hotel",AddressType.Billing,"CO", "80517")
                },
                ContactDetails = new ContactDetails("jack.torrance@shining.com", "666-6666"),
                CardDetails  = new CardDetails("4104231121998973", "517"),
                CurrentVersion = 1,
                Version = Account.SnapShot
            };

            var accountRepository = new UnitOfWork(Client);

            await accountRepository.SaveAsync(account);

            await Task.Delay(50);
            
            //act

            await accountRepository.DeleteAsync(Guid.Parse(account.AccountId), Account.SnapShot);

            await Task.Delay(50);
            
            var deletedAccount = await accountRepository.GetAsync(Guid.Parse(account.AccountId));

            //assert
            Assert.IsNull(deletedAccount);
            
        }

        protected override CreateTableRequest CreateTableRequest()
       {
            var createTableRequest = new DynamoDbTableFactory().GenerateCreateTableMapper<Account>(
                new DynamoDbCreateProvisionedThroughput(
                    new ProvisionedThroughput {ReadCapacityUnits = 10, WriteCapacityUnits = 10},
                    new Dictionary<string, ProvisionedThroughput>()
                ));
            return createTableRequest;
 
       }
    }
}