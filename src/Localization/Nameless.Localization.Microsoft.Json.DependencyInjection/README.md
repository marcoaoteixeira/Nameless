# Nameless Localization Microsoft JSON Library (DI)

Localization Microsoft JSON library (DI).

## Content

Dependency injection for Localization Microsoft JSON services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<LocalizationModule>();

var container = containerBuilder.Build();

var factory = container.Resolve<IStringLocalizerFactory>();
```