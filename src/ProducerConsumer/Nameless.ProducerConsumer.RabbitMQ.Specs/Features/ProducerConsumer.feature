Feature: Producer/Consumer
Simple message publisher / subscriber

Link to a feature: [ProducerConsumer](Nameless.ProducerConsumer.RabbitMQ.Specs/Features/ProducerConsumer.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

@RunsOnDevMachine
Scenario: Produce And Consume Message
	Given that I have the correct infrastructure for sending messages
	And the message value will be "This is a test message"
	When I call ProducerService ProduceAsync with that said message
	Then the handler created by ConsumerService Register should capture the message