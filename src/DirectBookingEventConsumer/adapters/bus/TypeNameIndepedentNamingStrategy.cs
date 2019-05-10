using System;
using JustRooms.DirectBookingEventConsumer.ports.events;
using JustSaying;
using JustSaying.AwsTools.QueueCreation;

namespace JustRooms.DirectBookingEventConsumer.adapters.bus
{
    public class TypeNameIndepedentNamingStrategy : INamingStrategy
    {
        public string GetTopicName(string baseTopicName, Type messageType)
        {
            if (messageType == typeof(RoomBookingMade))
            {
                return baseTopicName + "guestroombookingmade";
            }
            throw new ArgumentOutOfRangeException($"No SNS Topic registered for {nameof(messageType)}");
        }

        public string GetQueueName(SqsReadConfiguration sqsConfig, Type messageType)
        {
            if (string.IsNullOrWhiteSpace(sqsConfig.BaseQueueName))
            {
                if (messageType == typeof(RoomBookingMade))
                {
                    return "roombookingmade";
                }
            }
            else
            {
                return sqsConfig.BaseQueueName.ToLower();
            }
            
            throw new ArgumentOutOfRangeException($"No SQS Queue registered for {nameof(messageType)}");
         }

    }
}