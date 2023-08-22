# Nameless Caching Redis Library

Redis caching library.

## Content

### Classes
- _ConnectionMultiplexerManager_
- _RedisCache_
- _RedisOptions_
- _Root_
    - _EnvTokens_

### Interfaces
- _IConnectionMultiplexerManager_

## How To Use It

e.g.:

```
// From StackExchange.Redis
var opts = new ConfigurationOptions();
var connectionMultiplexer = ConnectionMultiplexer.Connect(opts)
var database = connectionMultiplexer.GetDatabase();

// From this library
var cache = new RedisCache(database);
```