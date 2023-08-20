# Nameless Autofac

[Autofac](https://autofac.readthedocs.io/en/latest/index.html) extension library.

## Content

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
- Not marked with SingletonAttribute (Nameless.Core)
- Should be assignable from (or generic) the given type.

**_PropertyResolveMiddleware_**: Useful to inject properties, using a factory.