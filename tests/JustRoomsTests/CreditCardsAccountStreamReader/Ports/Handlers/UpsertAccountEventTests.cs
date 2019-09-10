using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using CreditCardCore.Adapters.Data;
using CreditCardCore.Application;
using CreditCardCore.Ports.Events;
using CreditCardCore.Ports.Handlers;
using CreditCardCore.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using EFUnitOfWork = CreditCardCore.Adapters.Data.EFUnitOfWork;

namespace JustRoomsTests.CreditCardsAccountStreamReader.Ports.Handlers
{
    [TestFixture]
    public class UpsertAccountEventTests
    {
        private DbContextOptions<CardDetailsContext> _options;

        [SetUp]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<CardDetailsContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
       }
       
       [Test]
        public async Task When_adding_an_account()
        {
            using (var uow = new CardDetailsContext(_options))
            {
                //arrange
                var handler = new UpsertAccountEventHandler(_options);
                var @event = new UpsertAccountEvent()
                {
                    AccountId = Guid.NewGuid().ToString(),
                    Name = new Name {FirstName = "Jack", LastName = "Torrance"},
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            FistLineOfAddress = "Overlook Hotel",
                            AddressType = "Billing",
                            State = "CO",
                            ZipCode = "80517"
                        },
                        new Address
                        {
                            FistLineOfAddress = "3 Kennebunkport Avenue",
                            AddressType = "Home",
                            State = "MN",
                            ZipCode = "40125"
                        }
                    },
                    ContactDetails = new ContactDetails
                        {Email = "jack.torrance@shining.com", TelephoneNumber = "666-6666"},
                    CardDetails = new CardDetails {CardNumber = "4104231121998973", CardSecurityCode = "517"},
                    Version = 1
                };

                //act
                await handler.HandleAsync(@event);

                var repository = new AccountCardDetailsRepositoryAsync(new EFUnitOfWork(uow));
                var accountCardDetails = await repository.GetAsync(Guid.Parse(@event.AccountId));

                //assert
                Assert.That(accountCardDetails.AccountId, Is.EqualTo(@event.AccountId.ToString()));
                Assert.That(accountCardDetails.Name, Is.EqualTo(@event.Name.FirstName + " " + @event.Name.LastName));
                var sourceAddress = @event.Addresses.First(addr => addr.AddressType == "Billing");
                Assert.That(accountCardDetails.FirstLineOfAddress, Is.EqualTo(sourceAddress.FistLineOfAddress));
                Assert.That(accountCardDetails.ZipCode, Is.EqualTo(sourceAddress.ZipCode));
            }
        }

        [Test]
        public async Task When_updating_an_account()
        {
            using (var uow = new CardDetailsContext(_options))
            {
                //arrange
                var cardDetails = new AccountCardDetails(
                    accountId: Guid.NewGuid(),
                    name: "Jack Torrance",
                    cardNumber: "4104231121998973",
                    cardSecurityCode: "517",
                    firstLineOfAddress: "Overlook Hotel",
                    zipCode: "40125"
                );

                var repository = new AccountCardDetailsRepositoryAsync(new EFUnitOfWork(uow));
                
                await repository.AddAsync(cardDetails);

                var @event = new UpsertAccountEvent()
                {
                    AccountId = cardDetails.AccountId.ToString(),
                    Name = new Name {FirstName = "Charles", LastName = "Grady"},
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            FistLineOfAddress = "Overlook Hotel",
                            AddressType = "Billing",
                            State = "CO",
                            ZipCode = "80517"
                        }
                    },
                    ContactDetails = new ContactDetails
                        {Email = "charles.grady@shining.com", TelephoneNumber = "666-6666"},
                    CardDetails = new CardDetails {CardNumber = "4172097052597788", CardSecurityCode = "459"},
                    Version = 2
                };

                var handler = new UpsertAccountEventHandler(_options);

                //act
                await handler.HandleAsync(@event);

                var accountCardDetails = await repository.GetAsync(Guid.Parse(@event.AccountId));

                //assert
                Assert.That(accountCardDetails.AccountId, Is.EqualTo(@event.AccountId.ToString()));
                Assert.That(accountCardDetails.Name, Is.EqualTo(@event.Name.FirstName + " " + @event.Name.LastName));
                var sourceAddress = @event.Addresses.First(addr => addr.AddressType == "Billing");
                Assert.That(accountCardDetails.FirstLineOfAddress, Is.EqualTo(sourceAddress.FistLineOfAddress));
                Assert.That(accountCardDetails.ZipCode, Is.EqualTo(sourceAddress.ZipCode));
            }
        }
  

    }
}