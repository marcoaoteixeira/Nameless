Feature: ProducerConsumer
Producer/Consumer for RabbitMQ

Link to a feature: [ProducerConsumer](Nameless.ProducerConsumer.RabbitMQ.Specs/Features/ProducerConsumer.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

@ProducerConsumer
Scenario: Produce And Consume Message
	Given that I have a ChannelFactory
	And that I have a Channel
	And that I have a ProducerService
	And that I have a ConsumerService
	And that I create a Registration using ConsumerService
	When I use the ProducerService to publish a message with content This is a test message
	Then the Handler associated with the Registration should capture the message