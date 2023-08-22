# Nameless Caching In-Memory Library (DI)

In-Memory caching library (DI)

## Content

Dependency injection for In-Memory Cache

### How To Use It

e.g.:

```
var containerBuilder = new ContainerBuilder(); // Autofac

containerBuilder.RegisterModule<CachingModule>();

var container = containerBuilder.Build();
var cache = container.Resolve<ICache>();
var options = new CacheEntryOptions {
    ExpiresIn = TimeSpan.FromSeconds(60),
    EvictionCallback = (key, value, reason) => {
        // Deal with object evition here
    }
};
var result = await cache.SetAsync("KEY", value, options, cancellationToken);
```