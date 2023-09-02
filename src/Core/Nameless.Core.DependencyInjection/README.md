# Nameless Core (Dependency Injection) Documentation

## Content

Dependency injection for Core services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<CoreModule>();

var container = containerBuilder.Build();

var clockService = container.Resolve<IClockService>();
var xmlSchemaValidator = container.Resolve<IXmlSchemaValidator>();
// etc...
```