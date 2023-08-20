# Nameless Messenger E-mail Library (DI)

Messenger E-mail library (DI).

## Content

Dependency injection for messenger e-mail services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<MessengerModule>();

var container = containerBuilder.Build();

var messenger = container.Resolve<IMessenger>();
```