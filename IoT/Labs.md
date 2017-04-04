## 1. Getting started with IoT Hub, sending and receiving device messages
Here's the [link to the tutorial](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted).  
This takes roughly 45 to 60 minutes.

## 2. Sending cloud to device messages
Here's the [link to the tutorial](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-c2d).  
This can be done in less than 30 minutes, but it builds on the prior lab.

## 3. Processing IoT Hub messages at scale using the EventProcessorHost
Here's the [link to the tutorial](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-dotnet-standard-getstarted-receive-eph).  
Note that this tutorial focuses on Event Hubs, but IoT Hubs provide an Event Hub-compatible endpoint.  Because you already created an IoT Hub you can skip ahead to the step entitled "Create a console application."  When the lab asks for the Event Hubs connection string, go to your IoT Hub in the Azure portal, then go to the endpoints tab in the menu and find the Event Hub-compatible connection string.
This lab should take from 30 to 45 minutes.
