using System;
using System.Threading.Tasks;
using GuestDirectBookingContracts.publishing;
using MessageSchemaRepository.Verification;
using NUnit.Framework;

namespace DirectBookingEventConsumerContracts.Consume
{
    public class PublishConsumerContract
    {
        private const string MESSAGE_NAME = "guestroombookingmade";
        private const string BASE_DIR = @"/Users/ian.cooper/CSharpProjects/github/iancooper/JustRooms/tests/DirectBookingEventConsumerContracts/directbookingeventconsumer/contracts/";
        
        [Test]
        public async Task ProducerSchemaSupportsConsumerContract()
        {
            var publisher = new ContractPublisher(Environment.GetEnvironmentVariable("EVENT_CONTRACTS_REPO"), "guestdirectbooking");
            
            try
            {
                //publish the contract
                var joiSchema = await ContractReader.ReadLocalSchema(
                    BASE_DIR, 
                    MESSAGE_NAME, 
                    "direct-booking-consumer_guest-room-booking-made-schema-v1.0.0.js");
                await publisher.PublishContract(MESSAGE_NAME, joiSchema, "direct-booking-consumer_guest-room-booking-made-schema-v1.0.0.js");
                
                using (var verifier = new ContractVerifier())
                {
                    (bool HasErrors, string Errors) verifySchema = await verifier.GetVerificationResults(
                        new Uri("http://localhost:5000/"),
                        "guestdirectbooking/contracts/guestroombookingmade/direct-booking-consumer_guest-room-booking-made-schema-v1.0.0");
                    Assert.IsFalse(verifySchema.HasErrors, verifySchema.Errors);
                }
                
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
 
        }
    }
}