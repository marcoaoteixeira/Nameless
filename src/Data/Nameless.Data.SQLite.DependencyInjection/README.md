# Nameless Data SQL Server Library (DI)

Data SQL Server library (DI).

## Content

Dependency injection for Core services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<DataModule>();

var container = containerBuilder.Build();

var database = container.Resolve<IDatabase>();
```