# Nameless FluentValidation

FluentValidation library.

## Starting

Instructions below will show your the way to get things working.

### Pre-requirements

- [dotnet SDK](https://dotnet.microsoft.com/en-us/download)

### Installing

You'll need to install some apps to run this solution smoothly. Some
recomendations:

- [Microsoft Visual Studio Community](https://visualstudio.microsoft.com/downloads/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [DBeaver Community](https://dbeaver.io/download/)
- [Git](https://git-scm.com/downloads/)
- [Postman](https://www.postman.com/downloads/)
- [Notepad++](https://notepad-plus-plus.org/downloads/)
- [LINQPad](https://www.linqpad.net/Download.aspx) (For Windows-users only)
- [ILSpy](https://github.com/icsharpcode/ILSpy) (great tool when you need to
take a peek into some compiled code)

### Testing

There are some test projects, inside the "test" folder. Maybe you'll need to
install the coverage tool and a report tool. If I'm not mistaken, Visual Studio
alread has those dependencies installed for you after restore a test project.
But...

*Coverlet*
```
dotnet tool install -g coverlet.console
```

*.NET Report Generator*
```
dotnet tool install -g dotnet-reportgenerator-globaltool
```

## Coding Styles

Nothing written into stone, use your good sense. But you can refere to this
page, if you like: [Common C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

### Some Ideas to Keep in Mind

- When I create a new extension method I never check for nullability the target
    instance. If the target instance is _null_, you'll get an
    _NullReferenceException_ on your face, punk!

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

- Remember that when returning an _IEnumerable\<T\>_ it's a good practice to
use the _yield_ keyword to return each item.

## Deployment

GitHub Actions. See _.github_ folder.

## Contribuition

Just me, at the moment.

## Versioning

Using [SemVer](http://semver.org/) for assembly versioning.

## Authors

* **Marco Teixeira (marcoaoteixeira)** - *initial work*

## License

MIT

## Acknowledgement

* Hat tip to anyone whose code was used. 