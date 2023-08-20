# Nameless Lucene Library (DI)

Lucene library (DI).

## Content

Dependency injection for Lucene services.

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<LuceneModule>();

var container = containerBuilder.Build();

var provider = container.Resolve<IIndexProvider>();
```