# Nameless Security Library (DI)

Security library (DI).

## Content

Dependency injection for Core services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<SecurityModule>();

var container = containerBuilder.Build();

var passwordGenerator = container.Resolve<IPasswordGenerator>();
```