# Nameless Caching In-Memory Library

In-Memory caching library.

## Content

This library was written to be a wrapper over caching solutions
and provide a common API. So, that said...

## How To Use It

e.g.:

```
// From Microsoft.Extensions.Caching.Memory
var memoryCacheOptions = new MemoryCacheOptions();
var memoryCache = new MemoryCache(memoryCacheOptions);

// From this library
var cache = new InMemoryCache(memoryCache);
```