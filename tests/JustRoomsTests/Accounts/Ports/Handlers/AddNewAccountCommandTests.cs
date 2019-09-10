using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Handlers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace JustRoomsTests.Accounts.Ports.Handlers
{
    [TestFixture]
    public class AddNewAccountCommandTests
    {
        private DbContextOptions<AccountContext> _options;

        [SetUp]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<AccountContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
       }

        [Test]
        public async Task When_adding_an_account()
        {
            //arrange
            var commandProcessor = new FakeCommandProcessor();
            var handler = new AddNewAccountHandlerAsync(_options, commandProcessor);
            var command = new AddNewAccountCommand()
            {
                Id = Guid.NewGuid(),
                Name = new Name("Jack", "Torrance"),
                Addresses = new List<Address>
                {
                    new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                },
                ContactDetails = new ContactDetails("jack.torrance@shining.com", "666-6666"),
                CardDetails = new CardDetails("4104231121998973", "517")
            };
            
            //act
            await handler.HandleAsync(command);

            var savedAccount = await new EFUnitOfWork(new AccountContext(_options)).GetAsync(command.Id);

            //assert
            Assert.That(savedAccount.AccountId, Is.EqualTo(command.Id.ToString()));
            Assert.That(savedAccount.Name.FirstName, Is.EqualTo(command.Name.FirstName));
            Assert.That(savedAccount.Name.LastName, Is.EqualTo(command.Name.LastName));
            var expectedAddress = command.Addresses.First();
            var savedAddress = savedAccount.Addresses.FirstOrDefault();
            Assert.That(savedAddress, Is.Not.Null);
            Assert.That(savedAddress.AddressType, Is.EqualTo(expectedAddress.AddressType));
            Assert.That(savedAddress.FistLineOfAddress, Is.EqualTo(expectedAddress.FistLineOfAddress));
            Assert.That(savedAddress.State, Is.EqualTo(expectedAddress.State));
            Assert.That(savedAddress.ZipCode, Is.EqualTo(expectedAddress.ZipCode));
            
            Assert.IsTrue(commandProcessor.RaiseAccountEvent);
            Assert.IsTrue(commandProcessor.AllSent);
         }
    }
}