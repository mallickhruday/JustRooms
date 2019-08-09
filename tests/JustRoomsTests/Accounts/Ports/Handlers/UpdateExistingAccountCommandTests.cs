using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Handlers;
using NUnit.Framework;

namespace JustRoomsTests.Accounts.Ports.Handlers
{
    [TestFixture]
    public class UpdateExistingAccountCommandTests
    {
        
        private InMemoryUnitOfWork _unitOfWork;

        [SetUp]
        public void Initialize()
        {
            _unitOfWork = new InMemoryUnitOfWork();
        }

        [Test]
        public async Task When_updating_an_account()
        {
            //arrange
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
            
            //create update command
            var updateCommand = new UpdateExistingAccountCommand
            {
                AccountId = id,
                Name = new Name("Here's", "Johnny!!!"),
                Addresses = new List<Address>
                {
                    new Address("Overlook Hotel", AddressType.Billing, "CO", "80517")
                },
                ContactDetails = new ContactDetails("jack.torrance@shining.com", "666-6666"),
                CardDetails = new CardDetails("4104231121998973", "517")
            };

            var handler = new UpdateExistingAccountCommandHandlerAsync(_unitOfWork);

            //act
            //issue update command
            await handler.HandleAsync(updateCommand);


            //assert
            //versions and change to current state
            var accountRepository = new AccountRepositoryAsync(_unitOfWork);
            var accountSnapshot = await accountRepository.GetAsync(id);
            Assert.That(accountSnapshot.AccountId, Is.EqualTo(id.ToString()));
            //did we update the snapshot
            Assert.That(accountSnapshot.Name.FirstName, Is.EqualTo(updateCommand.Name.FirstName));
            Assert.That(accountSnapshot.Name.LastName, Is.EqualTo(updateCommand.Name.LastName));
            //is the current version incremented
            Assert.That(accountSnapshot.CurrentVersion, Is.EqualTo(2));
            //But we still get the V0 record
            Assert.That(accountSnapshot.Version, Is.EqualTo("V0"));
            //Now get the inserted record, and check it exists
            var accountVersion = await accountRepository.GetAsync(id, "V2");
            Assert.IsNotNull(accountVersion);
            Assert.That(accountVersion.Name.FirstName, Is.EqualTo(updateCommand.Name.FirstName));
            Assert.That(accountVersion.Name.LastName, Is.EqualTo(updateCommand.Name.LastName));
            //And confirm the first item in our history is still there
            var accountOlderVersion = await accountRepository.GetAsync(id, "V1");
            Assert.IsNotNull(accountOlderVersion);
            Assert.That(accountOlderVersion.Name.FirstName, Is.EqualTo(account.Name.FirstName));
            Assert.That(accountOlderVersion.Name.LastName, Is.EqualTo(account.Name.LastName));
            

        }
    }
}