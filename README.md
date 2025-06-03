# Nameless .NET

## What's the idea?

This repository project—a collection of versatile and easy-to-use libraries
designed for reuse across a wide range of .NET projects. These libraries are
implementation-agnostic, offering flexibility while also including default
implementations to help you get started quickly.

Whether you're building a console app, a microservice, or anything in between,
these libraries are here to make your development smoother and more efficient.
We warmly invite you to explore the code—see what might be useful for your
own work, or even spot opportunities to make things better.

To make contributing easier, there is a pull request template to guide you
through the process.

Happy coding!

## Requirements

To work properly with the solution, you'll need to install, at least the apps
listed below, just follow the links:

- [.NET Core SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
- [Visual Studio Code](https://code.visualstudio.com/download)

Or if you have the license for the full version of Visual Studio, you can use
it instead of VS Code:

- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)

Another option is to use [Rider](https://www.jetbrains.com/rider/) from
JetBrains. It's a paid software, but not too expensive. The good thing about
Rider is that you can use it on Windows, Linux and MacOS. So, if you are a
cross-platform developer, you can use it on any OS. Also, it provides a
powerful set of tools for C# development, including code analysis, refactoring,
and debugging. It also has a built-in terminal and support for Docker.

## Starting Up

Before you begin, make sure to run the `instrumentation.sh` script located in
the `build` folder at the root of the solution. This script installs the
necessary tools required to run the tests and generate the code coverage
report. If you're running on Windows, you can use the
[Git](https://git-scm.com/downloads) Bash to execute the script.

It also installs [Cake](https://cakebuild.net/), a cross-platform build
automation system that uses a C# DSL. With Cake, you can write build scripts
in C# and execute them on any platform, ensuring a consistent experience
across the team. It's an excellent tool for automating your build process.

## ASP.NET Core Dependency

If your solution/project requires ASP.NET Core or has dependency on it, please
consider add the reference to the SDK instead of include the NuGet package.
In your `.csproj` add the ItemGroup below: 

```
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
```

## Tools & Resources

Here are a couple of apps that you may find useful when developing:

- [Git](https://git-scm.com/downloads): No need (I hope) for presentations. 
- [Notepad++](https://notepad-plus-plus.org/downloads/): You know what this is,
  don't you?
- [Postman](https://www.postman.com/downloads/): Useful to test Rest API calls.
  Similar to SoapUI, but it's much more easy to handle.
- [LINQPad](https://www.linqpad.net/download.aspx): For writing LINQ-Queries or
  use as a REPL.
- [ReSharper](https://www.jetbrains.com/resharper/): ReSharper by JetBrains is
  a Visual Studio extension that helps developers write cleaner, faster, and
  more efficient .NET code by providing smart code analysis, refactoring tools,
  and suggestions.
- [Sonarlint](https://www.sonarsource.com/products/sonarlint/): Analyze and
  suggest code modifications on-the-fly. Has a free-tier.
- [ILSpy](https://github.com/icsharpcode/ILSpy/releases): When you want to take
  a peek on the compiled code that no one have the source.
- [SMTP4Dev](https://github.com/rnwood/smtp4dev/releases): Small SMTP server
  that runs as a service on your machine, for send e-mail tests. Very useful.
- [DBeaver](https://dbeaver.io/download/): An alternative for databases
  management. Don't have all options for each specific database, but it's
  useful.

## Testing

Testing is not just a safety net—it’s a celebration of our code working as
intended! I believe great software is built on a foundation of reliable,
maintainable tests. Whether it's unit, integration, or end-to-end testing,
each test is a small but mighty step toward quality and confidence. Let's keep
the bar high, enjoy the process, and make testing a natural, joyful part of
our development flow!

### Running Tests

You can easly run all tests using the command below in your `Terminal` at the
solution root folder:

```
dotnet test
```

If you want to run a specific test, you can use the `--filter` option. For
example, if you want to run only the tests within a spefic category, you can
use the command below:

```
dotnet test --filter "Category=UnitTests"
```
Just remember to add the `Category` attribute to your test class or method.

If you're using a IDE like Visual Studio or Rider, you can run the tests from
the IDE itself. Just open the test explorer and run all tests from there.

### Running Tests with Code Coverage

A simple command to run all tests with code coverage is:

```
dotnet test --logger:"Html;LogFileName=/code_coverage/code_coverage_log.html" --collect:"XPlat Code Coverage" --results-directory ./code_coverage/test_results --verbosity normal
```

Let's break it down:

- `dotnet test`: This command runs the tests in the current project.
- `--logger:"Html;LogFileName=/code_coverage/code_coverage_log.html"`: This
  option specifies the logger to use. In this case, we're using the HTML
  logger and specifying the log file name.
- `--collect:"XPlat Code Coverage"`: This option specifies that we want to
  collect code coverage data.
- `--results-directory ./code_coverage/test_results`: This option specifies
  the directory where the test results will be saved.
- `--verbosity normal`: This option specifies the verbosity level of the
  output. You can change it to `quiet`, `minimal`, `normal`, `detailed` or
  `diagnostic` if you want more or less information.

On one last note, we recommend you to become more familiar with
[.NET Core CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Go to
the website and read the documentation. It's really good and has a lot of
information. You can also use the `dotnet --help` command to get a list of all
available commands and options.

### Creating Code Coverage Report

If you don't have access to an IDE that can generate code coverage reports,
you can use the `ReportGenerator` tool to generate it. After running the tests
with code coverage, you can generate a report using the command below:

```
reportgenerator "-reports:./code_coverage/**/coverage.cobertura.xml" "-targetdir:./code_coverage/report" -reporttypes:Html
```

Again, let's break it down:
- `reportgenerator`: This command runs the ReportGenerator tool.
- `"-reports:./code_coverage/**/coverage.cobertura.xml"`: This option
  specifies the path to the coverage report file. The `**` wildcard is used
  to search for the file in all subdirectories.
- `"-targetdir:./code_coverage/report"`: This option specifies the
  directory where the report will be saved.
- `-reporttypes:Html`: This option specifies the type of report to
  generate. In this case, we're generating an HTML report.

You'll find the report inside the `code_coverage/report` folder. It's a simple
HTML report that you can open in any web browser.

If you want to know more about the report tool, go to
[ReportGenerator](https://reportgenerator.io/usage).

### Bonus

Now, as mentioned before, if you ran the `instrumentation.sh` script, you'll
get all of the above for free. All you need to do is run this command in your
`Terminal` at the root level of the solution:

```
dotnet cake .\build\build.cake --target CodeCoverage --working-dir ../ --rebuild
```
What this will do:
- Clean the solution
- Restore the NuGet packages
- Build the solution
- Run all tests
- Generate the code coverage report

Just in one go!

## Logging

To promote an easy way to identify log events, for specific log levels we add
the `EventId` to the log message. This way, you can easily filter the logs.
We'll attribute an range of `EventId`s to each projects, like:

- Core project: 100000-199999
- Autofac project: 200000-299999
- Data SQLite project: 300000-399999
- Data SQL Server project: 400000-499999


## Coding Styles

Nothing written into stone, use your ol'good common sense. But you can refere
to this page, if you like: [Common C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

### Some Ideas to Keep in Mind

- First things first, remember the basic rules of programming: **_KISS_** and
  **_DRY_**. Keep it simple and don't repeat yourself.
- YAGNI: **_You Are Gonna Need It_**. Don't create a class or method for
  something that you don't need. If you don't need it now, you probably won't
  need it in the future.
- SOLID: **_Single Responsibility Principle_**, **_Open/Closed Principle_**,
  **_Liskov Substitution Principle_**, **_Interface Segregation Principle_** and
  **_Dependency Inversion Principle_**. If you don't know what they are, go
  read about them. They are really important.
- When creating a new extension method, never check for nullability of the
  target instance. If the target instance is **_null_**, you'll get
  an **_NullReferenceException_**. This way, you can be sure that your code
  will behave properly.
- Not everything should be an extension method (or property).
- **_default_** vs **_null_** :
  - Am I sure is a reference type? **null**
  - Am I sure is a value type? **default**
  - I'm not sure at all: **default**
  - It's a _nullable_ value? **null**
- Prefer **_IEnumerable\<T\>_** over **_ICollection\<T\>_** or
  **_IList\<T\>_**. This way, you can return a collection of any type and
  avoid exposing the implementation details of your class.
- Methods or properties that returns arrays, collections or enumerables in
  general, **DO NOT RETURN _null_ VALUE, NEVER AND EVER!!!** If there's no
  value to be returned, just return an empty enumerable.
- If you're returning an **_IEnumerable\<T\>_**, probably you should use the
  **_yield_** keywork on your method return directive.
- Use the collection initializer syntax when creating a new collection,
  whenever is possible.
- Use object initializer, whenever is possible.
- Prefer **_var_** over explicit type declaration, whenever is possible.
- Prefer **_double_** over **_float_**. The only exception is when you're using
  **_float_** for a specific reason, like when you're working with a library
  that requires it.
- When dealing with currency values, prefer using **_decimal_** over
  **_double_** or **_float_**. This is because **_decimal_** has a higher
  precision and is more suitable for financial calculations.
- Use meaningful variable and method names – names should describe what the
  variable/method does or represents.
- Follow `PascalCase` for class, method, and property names, and `camelCase`
  for local variables and parameters.
- Avoid abbreviations and overly short names—clarity trumps brevity.
- Be consistent with indentation and formatting – we use the .editorconfig 
  in Visual Studio - or IDE settings to enforce.
- Keep methods short and focused – a method should do one thing well.
- Avoid deeply nested code – return early, use guard clauses
  Use `Prevent` class for that purpose.
- Comment why, not what – code should be self-explanatory; comments are for
  intent or edge cases.
- Group related code logically – regionally or using partial classes when
  necessary, but avoid as much as possible.
- Break down large classes – apply separation of concerns even within a single
  file.
- Always initialize variables properly – avoid uninitialized variables or
  `null` surprises.
- Avoid magic numbers and strings – use constants or enums.
- Use switch expressions or pattern matching where it improves readability.
- Handle all branches in conditionals – even if it's just a default case or
  exception.
- Be mindful of operator precedence – use parentheses to make logic clear.
- Use `using` statements or `using` declarations for disposable objects – avoid
  memory/resource leaks.
- Prefer `string` interpolation ($"...") over concatenation – it's more
  readable and less error-prone.
- Use null-coalescing (??) and null-conditional (?.) operators to avoid null
  reference exceptions.
- Favor `foreach` over `for` loops unless index access is required. Trick, you
  can use the extension method `ForEach` to do that, if you need a index
  number.
- Use `async` and `await` for asynchronous programming – avoid blocking
  calls.
- Write unit tests for critical logic – even basic tests can prevent major
  regressions.
- Use exceptions appropriately – don't swallow them silently or use them for
  control flow.
- Check for nulls before dereferencing objects – especially with external
  data or APIs.
- Avoid hardcoded file paths, URLs, and credentials – use config files or
  secrets management.

## Contribuition

List all the people that contributed to this project.

## Versioning

Using [SemVer](http://semver.org/) for assembly versioning.

## Authors

- **Marco Teixeira (marcoaoteixeira)** - _initial work_

## License

MIT

## Acknowledgement

- Hat tip to anyone who help expand this code base.