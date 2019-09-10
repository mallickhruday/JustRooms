# JustRooms

## Introduction
JustRooms is not built with specific knowledge of the hotel industry or how it function, not with accurate workflows for payment or security around PII etc. The domain chosen is designed to be
easy for anyone to reason about, because most of us have interacted with a hotel, over because we want to demonstrate
how to build a solution for that domain. By avoiding solving a 'real' problem we can focus on the variables at work in building an event driven solution.

 * Accounts: Holds guest account details
 * Credit Card Payments: Takes a payment for a booking
 * Direct Booking: Allows a guest to book a room at the hotel
 
 A given microservice may include multiple processes, usually web and worker.

## What are we showing

JustRooms tries to deme how microserviees collaborate using events. In particulary it tries to demonstrate some key patterns:

* [Event Carried State Transfer](https://martinfowler.com/articles/201701-event-driven.html)
    * We want to avoid making a request from one microservice to another using a synchronous request-response protocol because this creates temporal coupling between two services. Often the reason for the synchronous communication is that a 
    microservice wants to join its data with data from another microservice.
    * We can build a local cache to a 'downstream' microservice of the state of an 'upstream' microservice
    * We can then query this local cache instead of the 'upstream' service.
    * In Event Carried State Transfer we populate this cache not via caching the results of prior requests, but by listening
    to events produced by the upstream microservice that let us pre-emptively cache data in the downstream microservice
    * The cached data is 'Outside Data'. It is stale, immutable, and should be versioned. It should be a 'stable contract' with the upstream microservice not 
    implementation details.
    * In this example the **Accounts** microservice raises an event when a Guest Account is modified. The **Credit Card Payments** microservice
    caches card details for the customer raised via this microservice, so that it can take payments without calling the **Accounts** system.
    
* [Event Notification](https://martinfowler.com/articles/201701-event-driven.html)
    * We want to signal to a downstream system that something of interest has occured upstream.
    * The upstream system has a 'fire-and-forget' relationship to the notification it raises, it does not care who listens
    to the event of what they do with it.
    * Complex workflows can nevertheless be implemented by choreography, who listens to events and what events they raise in turn.
    * The **Direct Booking** microservice raises a notification when a booking has been made
    * The **Credit Card Payments** system listens for this event and tries to take a payment.
    
 
## Implementation Strategies

JustRooms has branches that show different strategies for Producer-Consumer Correctness and implementing Event Carried State Transfer

* dynamo-kinesis: demonstrates using Log Tailing for Producer-Consumer correctness and a message log for Event Carried State Transfer

