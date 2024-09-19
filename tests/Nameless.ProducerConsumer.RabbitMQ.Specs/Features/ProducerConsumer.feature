Feature: Producer/Consumer
Simple message publisher / subscriber

Link to a feature: [ProducerConsumer](Nameless.ProducerConsumer.RabbitMQ.Specs/Features/ProducerConsumer.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

@RunsOnDevMachine
Scenario: Produce And Consume Message
	Given there is a RabbitMQ instance configured
	When we produce a OrderStartedEvent message with and ID and Date
	Then the ConsumerService should receive the message and call its handler