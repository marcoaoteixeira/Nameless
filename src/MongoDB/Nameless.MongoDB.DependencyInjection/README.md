# Nameless MongoDB Library (DI)

MongoDB library (DI).

## Content

Dependency injection for MongoDB services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<MongoDBModule>();

var container = containerBuilder.Build();

var factory = container.Resolve<IStringLocalizerFactory>();
```