# BasicResults

Three small value types for modelling failure and absence in the type system rather than with
exceptions and nulls: `Result`, `Error` and `Maybe<T>`.

Targets `netstandard2.0` with no dependencies, so it runs on .NET 5 and later, .NET Core 2.0 and
later, and .NET Framework 4.7.2 and later. It can be consumed from C# 7.3 upwards. The test suite
runs on .NET 8.

## Install

```bash
dotnet add package BasicResults
```

```csharp
using BasicResults;
```

## Error

A domain error as a value: compared, returned and pattern-matched like any other.

```csharp
public static class UserErrors
{
    public static readonly Error NotFound = new Error("User.NotFound", "No user has that id.");
}
```

The `Code` is what callers compare against: a stable `Area.Reason` identifier. The `Description`
is for humans. `Error.None` is the sentinel for "no error" and is only ever valid on a success.
Building a failure from it throws, because a failure carrying "no error" is a success by another
name.

## Result

The outcome of an operation, with (`Result<T>`) or without (`Result`) a value.

```csharp
public static Result<int> ParseAge(string input)
{
    if (!int.TryParse(input, out var age) || age < 0)
    {
        return new Error("Age.Invalid", "Age must be a whole number, zero or more."); // Error -> failure
    }

    return age; // value -> success
}
```

The implicit conversion from a value does not apply when `T` is an interface, because C# does
not allow user-defined conversions from interface types. Return `Result.Success(value)` there.

Reading the wrong side throws rather than handing back a default (`Value` on a failure, `Error`
on a success), so a missing `IsSuccess` check fails loudly at the point of the mistake. The
message from `Value` names the error:

```
Cannot read Value from a failed result (Age.Invalid: Age must be a whole number, zero or more.). Check IsSuccess first.
```

`Result.Success(value)` rejects null: a success has a value by definition, and an optional one is
`Maybe<T>`'s job.

`Map` projects the value of a success and passes a failure through; `Bind` chains an operation
that may itself fail. Neither runs its delegate on a failure.

```csharp
Result<string> label = ParseAge(input)
    .Bind(age => RequireAdult(age))   // RequireAdult returns Result<int>
    .Map(age => "Age " + age);
```

## Maybe

An optional value, where absence is legal and expected. Absence that means something went wrong
is a `Result`, not a `Maybe`.

```csharp
Maybe<string> nickname = user.Nickname == null
    ? Maybe<string>.None()
    : Maybe<string>.Some(user.Nickname);

string shown = nickname.GetValueOrDefault(user.Name);
```

`Some` rejects null, `default` is `None()`, and equality is structural: two empties are equal, and
two `Some` are equal when their values are. `ToResult(error)` converts where an optional value
becomes a required one:

```csharp
Result<string> required = nickname.ToResult(new Error("User.NoNickname", "This user has no nickname."));
```

## Building

Requires the .NET 8 SDK or later.

```bash
dotnet build
dotnet test
```

## Licence

MIT. See [LICENSE](LICENSE).
