# Nameless FluentValidation Library (DI)

FluentValidation library (DI).

## Content

Dependency injection for Core services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<FluentValidationModule>();
// or
containerBuilder.RegisterModule(new FluentValidationModule(/* Array of Assemblies */));
// or
containerBuilder.AddFluentValidation(/* Array of Assemblies */);

var container = containerBuilder.Build();

var database = container.Resolve<IValidatorManager>();
```