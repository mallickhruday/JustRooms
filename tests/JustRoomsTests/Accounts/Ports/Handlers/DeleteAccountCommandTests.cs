using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Handlers;
using Accounts.Ports.Repositories;
using NUnit.Framework;
using Paramore.Darker;

namespace JustRoomsTests.Accounts.Ports.Handlers
{
    [TestFixture]
    public class DeleteAccountCommandTests
    {
        private InMemoryUnitOfWork _unitOfWork;

        [SetUp]
        public void Initialize()
        {
            _unitOfWork = new InMemoryUnitOfWork();
        }

        [Test]
        public async Task When_deleting_a_record()
        {
            //Add new account directly via repository not handler (needs both entries so no unit of work)
            var id = Guid.NewGuid();
            var account = new Account()
            {
                AccountId = id.ToString(),
                Name = new Name("Jack", "Torrance"),
                Addresses = new List<Address>
                {
                    new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                },
                ContactDetails = new ContactDetails("jack.torrance@shining.com", "666-6666"),
                CardDetails = new CardDetails("4104231121998973", "517"),
                CurrentVersion = 0,
                Version = "V0"
            };

            var rhs = new Account(Guid.Parse(account.AccountId), account.Name, account.Addresses, account.ContactDetails, account.CardDetails);
            rhs.CurrentVersion = account.CurrentVersion + 1;
            rhs.Version = Account.VersionPrefix + $"{rhs.CurrentVersion}";
            var copiedAccount = rhs;
            account.CurrentVersion = copiedAccount.CurrentVersion;

            await _unitOfWork.SaveAsync(copiedAccount);
            await _unitOfWork.SaveAsync(account);
            
            //create delete command
            var deleteCommand = new DeleteExistingAccountCommand(id);
            
            //delete the account
            var handler = new DeleteExistingAccountCommandHandlerAsync(_unitOfWork);
            await handler.HandleAsync(deleteCommand);
            
            //assert
            var repo = new AccountRepositoryAsync(_unitOfWork);
            var savedAccount = await repo.GetAsync(id);
            Assert.IsNull(savedAccount);
            var historicalVersion = await repo.GetAsync(id, "V1");
            Assert.IsNotNull(historicalVersion);
            Assert.That(historicalVersion.AccountId, Is.EqualTo(id.ToString()));
            Assert.That(historicalVersion.CurrentVersion, Is.EqualTo(1));
            Assert.That(historicalVersion.Version, Is.EqualTo("V1"));
        }
    }
}