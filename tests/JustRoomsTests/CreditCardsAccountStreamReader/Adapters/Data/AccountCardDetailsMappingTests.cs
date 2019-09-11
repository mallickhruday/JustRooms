using System;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using CreditCardCore.Adapters.Data;
using CreditCardCore.Application;
using CreditCardCore.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using EFUnitOfWork = CreditCardCore.Adapters.Data.EFUnitOfWork;

namespace JustRoomsTests.CreditCardsAccountStreamReader.Adapters.Data
{
    public class AccountCardDetailsMappingTests
    {
        
        private DbContextOptions<CardDetailsContext > _options;

        [SetUp]
        public void Initialise()
        {
            _options = new DbContextOptionsBuilder<CardDetailsContext >()
                .UseMySql( "Server=localhost;Uid=root;Pwd=root;Database=CardDetails")
                .Options;
            
            using (var context = new CardDetailsContext(_options))
            {
                context.Database.EnsureCreated();
            }
        }

        [Test]
        public async Task When_adding_card_details()
        {
            using (var uow = new CardDetailsContext(_options))
            {
                //arrange
                var cardDetails = new AccountCardDetails(
                    Guid.NewGuid(),
                    "Jack TOrrance",
                    "4104231121998973",
                    "517",
                    "3 Kennebunkport Avenue",
                    "OH");

                var repository = new AccountCardDetailsRepositoryAsync(new EFUnitOfWork(uow));

                //act
                await repository.AddAsync(cardDetails);

                var savedRecord = await repository.GetAsync(cardDetails.AccountId);
                
                //assert
                Assert.That(savedRecord.AccountId, Is.EqualTo(cardDetails.AccountId));
                Assert.That(savedRecord.Name, Is.EqualTo(cardDetails.Name));
                Assert.That(savedRecord.CardNumber, Is.EqualTo(cardDetails.CardNumber));
                Assert.That(savedRecord.CardSecurityCode, Is.EqualTo(cardDetails.CardSecurityCode));
                Assert.That(savedRecord.FirstLineOfAddress, Is.EqualTo(cardDetails.FirstLineOfAddress));
                Assert.That(savedRecord.ZipCode, Is.EqualTo(cardDetails.ZipCode));
            }
        }
     }
}