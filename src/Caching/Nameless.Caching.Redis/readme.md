# Nameless Caching Redis

Cache implementation of Nameless.Cache.Abstractions, under the hood it uses
[_NRedisStack_](https://github.com/redis/NRedisStack).

_I tried to mention every person that I got something from. If you find code
that needs to be given the correct authorship, please, let me know._

## Starting

Instructions below will show your the way to get things working.

### Pre-requirements

```
No pre-requirements
```

### Installing

```
Well, no need to install anything.
```

### Testing

There is a test projects. Maybe you'll need to install the coverage tool and a
report tool. If I'm not mistaken, Visual Studio already has those dependencies
installed for you after restore. But...

_.NET Coverlet Tool_

```
dotnet tool install -g coverlet.console
```

_.NET Report Generator Tool_

```
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Coding Styles

Nothing defined, use your good sense.

## Deployment

I'm using GitHub Actions to act as a CI/CD. All files are located in the
.github folder.

## Contribuition

Just me, at the moment.

## Versioning

Using [SemVer](http://semver.org/) for assembly versioning.

## Authors

- **Marco Teixeira (marcoaoteixeira)** - _initial work_

## License

MIT

## Acknowledgement

- Hat tip to anyone whose code was used.