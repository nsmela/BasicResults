namespace BasicResults;

/// <summary>
/// An optional value. Absence here is legal and expected; it is not a failure.
/// Failure belongs to <see cref="Result{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the value that may be present.</typeparam>
public readonly struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T? _value;

    /// <summary>Whether a value is present.</summary>
    public bool HasValue { get; }

    /// <summary>Whether no value is present. The inverse of <see cref="HasValue"/>.</summary>
    public bool HasNoValue => !HasValue;

    private Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary>A <see cref="Maybe{T}"/> holding <paramref name="value"/>.</summary>
    /// <param name="value">The value. Must not be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public static Maybe<T> Some(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Use Maybe<T>.None() rather than wrapping null.");
        }

        return new Maybe<T>(value);
    }

    /// <summary>The empty <see cref="Maybe{T}"/>, which is also its default value.</summary>
    public static Maybe<T> None() => default;

    /// <summary>
    /// The value held.
    /// </summary>
    /// <exception cref="InvalidOperationException">No value is present.</exception>
    public T Value => HasValue
        ? _value!
        : throw new InvalidOperationException("Maybe<T> is empty. Check HasValue first.");

    /// <summary>The value held, or <paramref name="fallback"/> when empty.</summary>
    /// <param name="fallback">What to return when no value is present.</param>
    public T GetValueOrDefault(T fallback) => HasValue ? _value! : fallback;

    /// <summary>Projects the value when present; stays empty otherwise.</summary>
    /// <typeparam name="TOut">The projected value's type.</typeparam>
    /// <param name="map">The projection, run only when a value is present.</param>
    public Maybe<TOut> Map<TOut>(Func<T, TOut> map) =>
        HasValue ? Maybe<TOut>.Some(map(_value!)) : Maybe<TOut>.None();

    /// <summary>Chains an operation that may itself be empty.</summary>
    /// <typeparam name="TOut">The chained operation's value type.</typeparam>
    /// <param name="bind">The continuation, run only when a value is present.</param>
    public Maybe<TOut> Bind<TOut>(Func<T, Maybe<TOut>> bind) =>
        HasValue ? bind(_value!) : Maybe<TOut>.None();

    /// <summary>
    /// Turns absence into a failure, for the boundary where an optional value becomes required.
    /// </summary>
    /// <param name="error">The error to fail with when empty.</param>
    public Result<T> ToResult(Error error) =>
        HasValue ? Result<T>.Success(_value!) : Result<T>.Failure(error);

    /// <summary>Two <see cref="Maybe{T}"/> are equal when both are empty, or both hold equal values.</summary>
    /// <param name="other">The instance to compare against.</param>
    public bool Equals(Maybe<T> other) =>
        HasValue == other.HasValue && (!HasValue || EqualityComparer<T>.Default.Equals(_value!, other._value!));

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Maybe<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HasValue ? EqualityComparer<T>.Default.GetHashCode(_value!) : 0;

    /// <summary>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public static bool operator ==(Maybe<T> left, Maybe<T> right) => left.Equals(right);

    /// <summary>Whether <paramref name="left"/> and <paramref name="right"/> differ.</summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public static bool operator !=(Maybe<T> left, Maybe<T> right) => !left.Equals(right);

    /// <summary>Renders as <c>Some(value)</c> or <c>None</c>.</summary>
    public override string ToString() => HasValue ? $"Some({_value})" : "None";
}
