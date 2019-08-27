using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreditCardsAccountStreamReader.Adapters.Data;
using CreditCardsAccountStreamReader.Ports.Events;
using CreditCardsAccountStreamReader.Ports.Handlers;
using NUnit.Framework;

namespace JustRoomsTests.CreditCardsAccountStreamReader.Ports.Handlers
{
    [TestFixture]
    public class UpsertAccountEventTests
    {
        private InMemoryUnitOfWork _unitOfWork;
        
        [SetUp]
        public void Initialize()
        {
           _unitOfWork = new InMemoryUnitOfWork(); 
        }
        
       [Test]
        public async Task When_adding_an_account()
        {
            //arrange
            var handler = new UpsertAccountEventHandlerAsync(_unitOfWork);
            var @event = new UpsertAccountEvent()
            {
                AccountId = Guid.NewGuid().ToString(),
                Name = new Name{FirstName = "Jack", LastName = "Torrance"},
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
                        FistLineOfAddress= "3 Kennebunkport Avenue",
                        AddressType = "Home",
                        State = "MN",
                        ZipCode = "40125"
                    }
                },
                ContactDetails = new ContactDetails{Email = "jack.torrance@shining.com", TelephoneNumber = "666-6666"},
                CardDetails = new CardDetails{CardNumber = "4104231121998973", CardSecurityCode = "517"}
            };
            
            //act
            await handler.HandleAsync(@event);

            var accountCardDetails = await _unitOfWork.GetAsync(Guid.Parse(@event.AccountId));

            //assert
            Assert.That(accountCardDetails.AccountId, Is.EqualTo(@event.AccountId.ToString()));
            Assert.That(accountCardDetails.Name, Is.EqualTo(@event.Name.FirstName + " " + @event.Name.LastName));
            var sourceAddress = @event.Addresses.First(addr => addr.AddressType == "Billing");
            Assert.That(accountCardDetails.FirstLineOfAddress, Is.EqualTo(sourceAddress.FistLineOfAddress));
            Assert.That(accountCardDetails.ZipCode, Is.EqualTo(sourceAddress.ZipCode));
            Assert.That(accountCardDetails.Version, Is.EqualTo("V0"));
            Assert.That(accountCardDetails.CurrentVersion, Is.EqualTo(1));
         }
  

    }
}