using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Repositories;
using Amazon.DynamoDBv2.Model;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Outbox.DynamoDB;

namespace JustRoomsTests.Accounts.adapters.data
{
    [TestFixture]
    public class AccountMappingTests 
    {
        private DbContextOptions<AccountContext> _options;

        [SetUp]
        public void Initialise()
        {
            _options = new DbContextOptionsBuilder<AccountContext>()
                .UseMySql( "Server=localhost;Uid=root;Pwd=root;Database=Accounts")
                .Options;
            
            using (var context = new AccountContext(_options))
            {
                context.Database.EnsureCreated();
            }
 
        }
        
        [Test]
        public async Task When_adding_an_account()
        {
            using (var uow = new AccountContext(_options))
            {
                //arrange
                var id = Guid.NewGuid();
                var account = new Account()
                {
                   AccountId = id,
                   Addresses = new List<Address>
                    {
                        new Address("Overlook Hotel", AddressType.Work, "CO", "80517"),
                        new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                    },
                    LockedBy = "SYS",
                    LockExpiresAt = DateTime.Now.AddMilliseconds(500).Ticks.ToString()
                };
                account.Name = new Name(account, "Jack", "Torrance");
                account.ContactDetails = new ContactDetails(account, "jack.torrance@shining.com", "666-6666");
                account.CardDetails = new CardDetails(account, "4104231121998973", "517");

                var accountRepository = new AccountRepositoryAsync(new EFUnitOfWork(uow));

                //act
                await accountRepository.AddAsync(account);

                await Task.Delay(50);

                var savedAccount = await accountRepository.GetAsync(id);

                //assert
                Assert.That(savedAccount.AccountId, Is.EqualTo(id));
                Assert.That(savedAccount.Name.FirstName, Is.EqualTo(account.Name.FirstName));
                Assert.That(savedAccount.Name.LastName, Is.EqualTo(account.Name.LastName));
                Assert.That(savedAccount.Addresses.First().AddressType,
                    Is.EqualTo(account.Addresses.First().AddressType));
                Assert.That(savedAccount.Addresses.First().FistLineOfAddress,
                    Is.EqualTo(account.Addresses.First().FistLineOfAddress));
                Assert.That(savedAccount.Addresses.First().State, Is.EqualTo(account.Addresses.First().State));
                Assert.That(savedAccount.Addresses.First().ZipCode, Is.EqualTo(account.Addresses.First().ZipCode));
                Assert.That(savedAccount.ContactDetails.Email, Is.EqualTo(savedAccount.ContactDetails.Email));
                Assert.That(savedAccount.CardDetails.CardSecurityCode,
                    Is.EqualTo(savedAccount.CardDetails.CardSecurityCode));
                Assert.That(savedAccount.ContactDetails.Email, Is.EqualTo(savedAccount.ContactDetails.Email));
                Assert.That(savedAccount.ContactDetails.TelephoneNumber,
                    Is.EqualTo(account.ContactDetails.TelephoneNumber));
                Assert.That(savedAccount.LockedBy, Is.EqualTo(account.LockedBy));
                Assert.That(savedAccount.LockExpiresAt, Is.EqualTo(account.LockExpiresAt));
            }
        }

        [Test]
        public async Task When_deleting_an_account()
        {
            using (var uow = new AccountContext(_options))
            {
                var id = Guid.NewGuid();
                var account = new Account()
                {
                   AccountId = id,
                   Addresses = new List<Address>
                    {
                        new Address("Overlook Hotel", AddressType.Work, "CO", "80517"),
                        new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                    },
                    LockedBy = "SYS",
                    LockExpiresAt = DateTime.Now.AddMilliseconds(500).Ticks.ToString()
                };
                account.Name = new Name(account, "Jack", "Torrance");
                account.ContactDetails = new ContactDetails(account, "jack.torrance@shining.com", "666-6666");
                account.CardDetails = new CardDetails(account, "4104231121998973", "517");
 
                var accountRepository = new AccountRepositoryAsync(new EFUnitOfWork(uow));

                await accountRepository.AddAsync(account);

                await Task.Delay(50);

                //act

                await accountRepository.DeleteAsync(account.AccountId);

                await Task.Delay(50);

                var deletedAccount = await accountRepository.GetAsync(account.AccountId);

                //assert
                Assert.IsNull(deletedAccount);
            }
        }
   }
}