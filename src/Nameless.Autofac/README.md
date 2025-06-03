# Nameless Autofac Library

This library provides a set of extensions and utilities for the
[Autofac](https://autofac.readthedocs.io/en/latest/index.html) dependencyty
injection framework.

## Content

- [PropertyResolveMiddleware](#property-resolve-middleware)
- [ServiceProviderExtensions](#service-provider-extensions)
- [ComponentContextExtensions](#component-context-extensions)

<a name="property-resolve-middleware"></a>
## Property Resolve Middleware

Register the middleware in your Autofac container to enable property injection,
when constrting components. This is useful for scenarios where you want to
inject properties into components that are not directly registered in the
container or when injection through constructor is not available.

### How to Use

```csharp
var builder = new ContainerBuilder();

builder.RegisterType<ClassWithPropertyToInject>()
       .ConfigurePipeline(configure => {
           var propertyType = typeof(IServiceToInject);
           var resolver = (MemberInfo _, IComponentContext ctx) => ctx.Resolve<IServiceToInject>();
           
           configure.Use(new PropertyResolveMiddleware(propertyType, resolver));
       });
```

<a name="service-provider-extensions"></a>
## Service Provider Extensions

<a name="component-context-extensions"></a>
## Component Context Extensions

**_ModuleBase_**: This abstract class should be used when you want to create
an [_Autofac.Module_](https://autofac.readthedocs.io/en/latest/configuration/modules.html).

Methods:

- _GetImplementation\<TService\>_: Retrieves, from support assemblies, a single
implementation from the given service type.
- _GetImplementation(Type serviceType)_: Non-generic version of _GetImplementation\<TService\>_.
- _GetImplementations\<TService\>_: Retrieves, from support assemblies, all
implementations from the given service type.
- _GetImplementations(Type serviceType)_: Non-generic version of _GetImplementations\<TService\>_.

Note: All those methods searches for implementations that are:
- Exportable types
- Non-Interface
- Non-Abstract
- Should be assignable from (or generic) the given type.

**_PropertyResolveMiddleware_**: Useful to inject properties, using a factory.