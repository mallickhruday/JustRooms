using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Exceptions;
using Accounts.Ports.Handlers;
using Accounts.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace JustRoomsTests.Accounts.Ports.Handlers
{
    //TODO: We create account enough times that we might be better off with a Builder object that takes a UOW and inserts the correc records
    
    [TestFixture]
    public class UpdateExistingAccountCommandTests
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
        public async Task When_updating_an_account()
        {
            var commandProcessor = new FakeCommandProcessor();
            
            using (var uow = new AccountContext(_options))
            {
                var id = Guid.NewGuid();
                var account = new Account()
                {
                    AccountId = id,
                    Name = new Name("Jack", "Torrance"),
                    Addresses = new List<Address>
                    {
                        new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                    },
                    ContactDetails = new ContactDetails("jack.torrance@shining.com", "666-6666"),
                    CardDetails = new CardDetails("4104231121998973", "517"),
                };

                var repository = new AccountRepositoryAsync(new EFUnitOfWork(uow));
                
                await repository.AddAsync(account);

                //create update command
                var updateCommand = new UpdateExistingAccountCommand
                (
                    id,
                    new Name("Here's", "Johnny!!!"),
                    new List<Address>
                    {
                        new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                    },
                    new ContactDetails("jack.torrance@shining.com", "666-6666"),
                    new CardDetails("4104231121998973", "517")
                );

                var handler = new UpdateExistingAccountCommandHandlerAsync(_options, commandProcessor);

                //act
                //issue update command
                await handler.HandleAsync(updateCommand);

                //assert
                //versions and change to current state
                var updateAccount = await repository.GetAsync(id);
                Assert.That(updateAccount.AccountId, Is.EqualTo(id.ToString()));
                Assert.That(updateAccount.Name.FirstName, Is.EqualTo(updateCommand.Name.FirstName));
                Assert.That(updateAccount.Name.LastName, Is.EqualTo(updateCommand.Name.LastName));
            }
        }

        [Test]
        public async Task When_trying_to_update_a_locked_Account()
        {
            var commandProcessor = new FakeCommandProcessor();
            
            using (var uow = new AccountContext(_options))
            {
                //Add new account directly via repository not handler (needs both entries so no unit of work)
                var id = Guid.NewGuid();
                var account = new Account()
                {
                    AccountId = id,
                    Name = new Name("Jack", "Torrance"),
                    Addresses = new List<Address>
                    {
                        new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                    },
                    ContactDetails = new ContactDetails("jack.torrance@shining.com", "666-6666"),
                    CardDetails = new CardDetails("4104231121998973", "517"),
                };
                
                var repository = new AccountRepositoryAsync(new EFUnitOfWork(uow));

                await repository.AddAsync(account);

                //create update command
                var updateCommand = new UpdateExistingAccountCommand
                (
                    id,
                    new Name("Here's", "Johnny!!!"),
                    new List<Address>
                    {
                        new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                    },
                    new ContactDetails("jack.torrance@shining.com", "666-6666"),
                    new CardDetails("4104231121998973", "517")
                );
                updateCommand.LockBy = "GRADY";


                //Lock the existing account
                var aggregateLock = await repository.LockAsync(id.ToString(), "SYS");

                //now try to update whilst locked
                var handler = new UpdateExistingAccountCommandHandlerAsync(_options, commandProcessor);
                Assert.ThrowsAsync<CannotGetLockException>(async () => await handler.HandleAsync(updateCommand));

                //release the lock
                await aggregateLock.ReleaseAsync();

                //now we should be able to get the lock and update
                await handler.HandleAsync(updateCommand);

                var amendedAccount = await repository.GetAsync(id);
                Assert.That(amendedAccount.Name.FirstName, Is.EqualTo(updateCommand.Name.FirstName));
                
                Assert.IsTrue(commandProcessor.RaiseAccountEvent);
                Assert.IsTrue(commandProcessor.AllSent);
            }

        }
    }
}