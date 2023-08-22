# Nameless Producer/Consumer RabbitMQ (DI)

Producer/Consumer RabbitMQ library (DI).

## Content

Dependency injection for Producer/Consumer RabbitMQ services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<ProducerConsumerModule>();

var container = containerBuilder.Build();

var producer = container.Resolve<IProducerService>();
var consumer = container.Resolve<IConsumerService>();
// etc...
```