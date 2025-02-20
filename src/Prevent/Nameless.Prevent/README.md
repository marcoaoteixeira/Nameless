# Nameless Prevent

In computer programming, a guard is a Boolean expression that must evaluate to
true if the execution of the program is to continue in the branch in question.
Regardless of which programming language is used, a guard clause, guard code,
or guard statement is a check of integrity preconditions used to avoid errors
during execution. See more [Guard (Computer Science)](https://en.wikipedia.org/wiki/Guard_(computer_science)).

## Installation

Use NuGet package manager with your favorite source to install *Nameless.Prevent*

```powershell
dotnet add package Nameless.Prevent
```

## Usage

```csharp
public void AcknowledgeMessage(Message message) {
    // This will throw ArgumentNullException if parameter
    // "message" is null.
    Prevent.Argument.Null(message);

    ...your code
}
```
OR
```csharp
private readonly IUserRepository _repository;

public UserService(IUserRepository repository) {
    // This will throw ArgumentNullException if parameter
    // "repository" is null.
    _repository = Prevent.Argument.Null(repository);

    ...your code
}
```
There are many other guard clauses methods implemented. If you're using
.NET Core 6 or greater, you don't need to provide the parameter name argument
for the guard method. Since it can be retrieved through attribute [CallerArgumentExpressionAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerargumentexpressionattribute?view=net-9.0).

## Contributing

Pull requests are welcome. For major changes, please open an issue first to
discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)