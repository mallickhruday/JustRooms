using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Handlers;
using Accounts.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Paramore.Darker;

namespace JustRoomsTests.Accounts.Ports.Handlers
{
    [TestFixture]
    public class DeleteAccountCommandTests
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
        public async Task When_deleting_a_record()
        {
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

                //create delete command
                var deleteCommand = new DeleteExistingAccountCommand(id);

                //delete the account
                var handler = new DeleteExistingAccountCommandHandlerAsync(_options);
                await handler.HandleAsync(deleteCommand);

                //assert
                var savedAccount = await repository.GetAsync(id);
                Assert.IsNull(savedAccount);
            }
        }
    }
}