# Result Class

## Overview

The Result Pattern offers a structured approach to minimizing the use of
exceptions across our system. As you may know, exceptions are computationally
expensive for the framework to process�they can significantly impact
performance, especially when used excessively or inappropriately.

By adopting the Result Pattern, we shift from relying on exceptions for control
flow to using explicit success/failure indicators. This not only improves
performance but also enhances code readability and predictability. Developers
can handle outcomes more gracefully, making the system more robust and easier
to maintain.

In essence, the Result Pattern encourages us to treat errors as data rather
than exceptional events, allowing for cleaner logic and better separation of
concerns.

## How To Use It

In many systems, it's common to validate user input and throw exceptions when
the input is invalid. For example:

```csharp
public User GetUserByEmail(string email) {
	ArgumentException.ThrowIfNullOrWhiteSpace(email);
	
	// the code to get the user by email

	return user;
}
```

In this example, if the `email` parameter is `null`, empty, or consists only of
whitespace, an `ArgumentException` is thrown. While this approach is valid, it
comes with a performance cost�exceptions are expensive for the runtime to
handle and can clutter control flow when used excessively.

Instead, we can apply the Result class to handle such cases more gracefully
and efficiently:

```csharp
public Result<User> GetUserByEmail(string email) {
	if (string.IsNullOrWhiteSpace(email)) {
		return Error.Failure($"Paramter {nameof(email)} cannot be null, empty or only whitespaces.");
	}
	
	// the code to get the user by email

	return user;
}
```

In this version, we return a `Result<User>` object that encapsulates either a
successful outcome or an error message. This avoids throwing exceptions for
expected validation failures and makes the flow of the program more
predictable.

## Handling the Result

To process the result, we can use method `Match` to separate success and error
handling logic:

```csharp
var result = GetUserByEmail("user@enhesa.com");

result.Match(
	onSuccess: user => DoSomethingWithUserInstance(user),
	onFailure: errors => DoSomethingWithErrors(errors),
);

```

This approach promotes cleaner code, better separation of concerns, and
improved performance. It also makes it easier to compose and test business
logic without relying on exception handling for control flow.

## Extend `Result<T>`

You can extend the `Result<T>` class to your needs by creating custom
`Result<T>` types. For example, you might want to create a `CreateUserResult`
class that encapsulates the result of a user creation operation:

```csharp
public class CreateUserResult : Result<User> {
	// Use a private constructor to prevent the class for being
	// constructed. We should use the implicit operators
	// for that.
	private CreateUserResult(User result, Error[] errors)
		: base(result, errors) { }

	// Use implicit operators for easier conversions
	// from User and Error[] to CreateUserResult
	public static implicit operator CreateUserResult(User result) {
		return new CreateUserResult(result, errors: []);
	}

	public static implicit operator CreateUserResult(Error[] errors) {
		return new CreateUserResult(result: null, errors);
	}
}
```

## Notes

> :warning: The parameterless constructor is intentionally disabled; use the
implicit conversions or the protected constructor from derived types.

## References

- https://andrewlock.net/working-with-the-result-pattern-part-1-replacing-exceptions-as-control-flow/
- https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern
- https://www.linkedin.com/pulse/result-pattern-c-comprehensive-guide-andre-baltieri-wieuf


# Switch Class

## Overview

The `Switch<TArg0, TArg1>` class is a simple discriminated-union-like return
type. It represents a value that can be **one of two possible types** and
carries the active value with an **index** identifying which branch is set:

- `Index == 0` ? the value is of type `TArg0` (use `IsArg0`, `AsArg0`)
- `Index == 1` ? the value is of type `TArg1` (use `IsArg1`, `AsArg1`)
- Other values can be implemented in derived types.

## Members
- `bool IsArg0`, `bool IsArg1` - tell you which branch is active. They are
annotated with `[MemberNotNullWhen]` to aid nullable flow analysis.
- `TArg0 AsArg0`, `TArg1 AsArg1` - accessors that throw if you read the wrong
branch.
- `object? Value` - the active value as `object?`.
- `int Index` - the active branch index.
- `void Match(Action<TArg0> onArg0, Action<TArg1> onArg1)` - execute a
side-effect based on the active branch.
- `TResult Match<TResult>(Func<TArg0, TResult> onArg0, Func<TArg1, TResult> onArg1)` - execute a
side-effect based on the active branch and returns a result.
- Two **implicit conversions** so you can assign a `TArg0` or `TArg1` directly
to `Switch<TArg0, TArg1>`.

## How to Use

### Create a `Switch` Value

You typically rely on the implicit conversions:

```csharp
using Enhesa.Results;

Switch<string, int> s1 = "hello"; // Index=0, IsArg0=true
Switch<string, int> s2 = 42;       // Index=1, IsArg1=true
```

### Branch with `Match`

Prefer `Match` to avoid manual `if`/`else` and to keep handling exhaustive:

```csharp
s1.Match(
    onArg0: text => Console.WriteLine($"text length: {text.Length}"),
    onArg1: number => Console.WriteLine($"number squared: {number * number}")
);

var lengthOrDigits = s2.Match(
    onArg0: text => text.Length,
    onArg1: number => number.ToString().Length
);
```

### Guarded Accessors

If you need direct access to the value for a specific branch, check the guard
first:

```csharp
if (s1.IsArg0) {
    Console.WriteLine(s1.AsArg0.ToUpperInvariant());
} else if (s1.IsArg1) {
    Console.WriteLine(s1.AsArg1 + 1);
}
```

Attempting to read the wrong accessor throws `InvalidOperationException`.

### Minimal E2E Example

```csharp
using Enhesa.Results;

static Switch<string, int> TryParseInt(string input)
{
    if (int.TryParse(input, out var n))
    {
        return n; // implicit to Switch<string, int>
    }

    return input; // implicit to Switch<string, int>
}

var result = TryParseInt("1234");

var message = result.Match(
    onArg0: text => $"Not a number: '{text}'",
    onArg1: number => $"Parsed: {number}"
);

Console.WriteLine(message);
```

### Exceptions summary

- `InvalidOperationException` when:
  - Accessing `AsArg0` while `IsArg0 == false`.
  - Accessing `AsArg1` while `IsArg1 == false`.
  - `Value` is requested with an invalid `Index` (should not happen with
	provided factory paths).
  - The private parameterless constructor is used (guarded to prevent misuse).
  - `Match<TResult>` is called when neither branch matches (cannot occur with
	the provided implementation unless extended incorrectly).

## Extend `Switch<TArg0, TArg1>`

Unfortunately, there is not direct way to extend `Switch<TArg0, TArg1>` for
more branches. However, you can create your own derived types that add more
branches by implementing similar logic. You can see it in action in the
`Switch<TArg0, TArg1, TArg2>` class.

## Notes

> :warning: The parameterless constructor is intentionally disabled; use the
implicit conversions or the protected constructor from derived types.