# Nameless NHibernate Library (DI)

NHibernate library (DI).

## Content

Dependency injection for Core services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<NHibernateModule>();

var container = containerBuilder.Build();

var session = container.Resolve<ISession>();
```