# BasicResults

Three small value types for modelling failure and absence in the type system rather than with
exceptions and nulls: `Result`, `Error` and `Maybe<T>`.

They were written for [Fabolus](https://github.com/nsmela/Fabolus) and
[GeometryEngine](https://github.com/nsmela/GeometryEngine), which had grown a copy each. Two
copies of a type this foundational drift apart quietly — one gained `Map`/`Bind`, the other did
not, and a `Maybe<T>` without an `Equals` override falls back to reflective struct equality — so
they live here instead, in one place both can reference.

`net8.0`, no dependencies.

## Error

A domain error as a value: compared, returned and pattern-matched like any other.

```csharp
public static class MeshErrors
{
    public static readonly Error Empty = new("Mesh.Empty", "The mesh has no triangles.");
}
```

The `Code` is what callers compare against — a stable `Area.Reason` identifier. The `Description`
is for humans. `Error.None` is the sentinel for "no error" and is only ever valid on a success;
constructing a failure from it throws, because a failure carrying "no error" is a success by
another name.

## Result

The outcome of an operation, with (`Result<T>`) or without (`Result`) a value.

```csharp
public Result<IMesh> Union(IMesh a, IMesh b)
{
    if (a.IsEmpty || b.IsEmpty)
    {
        return MeshErrors.Empty;   // implicit: Error -> failure
    }

    return Combine(a, b);          // implicit: value -> success
}
```

Reading the wrong side throws rather than handing back a default — `Value` on a failure, `Error`
on a success — so a missing `IsSuccess` check fails loudly at the point of the mistake. The
message from `Value` names the error, which is usually the one thing you wanted to know:

```
Cannot read Value from a failed result (Mesh.Empty: The mesh has no triangles.). Check IsSuccess first.
```

`Success<T>(value)` rejects null: a success has a value by definition, and an optional one is
`Maybe<T>`'s job.

`Map` projects the value of a success and propagates the error of a failure; `Bind` chains an
operation that may itself fail. Neither runs its delegate on a failure.

```csharp
var volume = Import(path)
    .Bind(mesh => Repair(mesh))
    .Map(mesh => mesh.Volume);
```

## Maybe

An optional value, where absence is legal and expected. Absence that means something *went wrong*
is a `Result`, not a `Maybe`.

```csharp
var index = surface.HasValue
    ? Maybe<ISpatialIndex>.Some(BuildIndex(surface.Value))
    : Maybe<ISpatialIndex>.None();
```

`Some` rejects null, `default` is `None()`, and equality is structural: two empties are equal, and
two `Some` are equal when their values are. `ToResult(error)` converts at the boundary where an
optional value becomes a required one.

## Building

```bash
dotnet build
dotnet test
```

## Licence

MIT — see [LICENSE](LICENSE). Use it, change it, ship it, no attribution beyond keeping the
copyright notice with the source.
