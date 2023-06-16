# Nameless

This repository contains all code that I assume is useful in most of the cases
where I need to build something. So, if you think that it could be useful for
you, feel free to fork, clone, etc. Also, I tried to mention every person that
I got something from. If you find code that needs to be given the correct
authorship, please, let me know.

## Starting

Instructions below will show your the way to get things working.

### Pre-requirements

```
No pre-requirements
```

## Installing

```
Well, no need to install anything.
```

## Testing

There are some test projects. Maybe you'll need to install the coverage tool
and a report tool. If I'm not mistaken, Visual Studio already has those
dependencies installed for you after restore. But...

_.NET Coverlet Tool_

```
dotnet tool install -g coverlet.console
```

_.NET Report Generator Tool_

```
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Integration Tests

Some tests requires an environment to run properly, such as Redis Cache
or things like that. So, those tests have a category setted on them named
**_INTEGRATION_**. Make sure that the test running script ignore those
tests marked with this category so you don't break your build.

## Coding Styles

Nothing written into stone, use your good sense. But you can refere to this
page, if you like: [Common C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

### Some Ideas to Keep in Mind

- When I create a new extension method I never throw an _ArgumentNullException_
  if the target instance is _null_. What I do is to return the _default_ value.
  - The only cases are some where it's not possible to return a _default_ value,
    so I'll throw an excption. And that's it.
- **_default_** vs **_null_** :
  - Am I sure is a reference type? **null**
  - Am I sure is a value type? **default**
  - I'm not sure at all: **default**
- I think that most of my developer's life I spent writing C# code. But I like
  that kind of code organization from Java so, I use, almost, that same style
  of code identation on my project.
- Methods that returns arrays, collections or enumerables in general,
  **DO NOT RETURN NULL VALUE EVER!!!** If there is no value to return, just
  return an empty enumerable from the same type. Use [_Array.Empty\<T\>()_](https://learn.microsoft.com/en-us/dotnet/api/system.array.empty?view=net-7.0) or
  [_Enumerable.Empty\<T\>()_](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.empty?view=net-7.0)

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
