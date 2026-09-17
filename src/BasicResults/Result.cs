namespace BasicResults;

/// <summary>
/// The outcome of an operation that has no return value.
/// Failure is modelled in the type system rather than thrown.
/// </summary>
public class Result
{
    private readonly Error _error;

    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Whether the operation failed. The inverse of <see cref="IsSuccess"/>.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// The error describing the failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">The result is a success.</exception>
    public Error Error => IsSuccess
        ? throw new InvalidOperationException("Cannot read Error from a success result. Check IsFailure first.")
        : _error;

    /// <summary>Initialises a result. Derived types decide which states they expose.</summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The failure's error, or <see cref="Error.None"/> on a success.</param>
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        _error = error;
    }

    /// <summary>A success carrying no value.</summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>A success carrying <paramref name="value"/>.</summary>
    /// <typeparam name="T">The value's type.</typeparam>
    /// <param name="value">The value produced. Must not be null.</param>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>A failure described by <paramref name="error"/>.</summary>
    /// <param name="error">The error. Must be neither null nor <see cref="Error.None"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="error"/> is <see cref="Error.None"/>.</exception>
    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        if (error == Error.None)
        {
            throw new ArgumentException("A failure needs a meaningful error, not Error.None.", nameof(error));
        }

        return new Result(false, error);
    }

    /// <summary>A failure of a value-bearing result, described by <paramref name="error"/>.</summary>
    /// <typeparam name="T">The value type the result would have carried.</typeparam>
    /// <param name="error">The error. Must be neither null nor <see cref="Error.None"/>.</param>
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);

    /// <summary>Lets a method return an <see cref="BasicResults.Error"/> where a failure is expected.</summary>
    /// <param name="error">The error to wrap.</param>
    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// The outcome of an operation that either yields a value or a domain error.
/// </summary>
/// <typeparam name="T">The type of the value carried by a success.</typeparam>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// The value produced by a success.
    /// </summary>
    /// <exception cref="InvalidOperationException">The result is a failure; the message names the error.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot read Value from a failed result ({Error}). Check IsSuccess first.");

    private Result(T value) : base(true, Error.None) => _value = value;
    private Result(Error error) : base(false, error) => _value = default;

    /// <summary>A success carrying <paramref name="value"/>.</summary>
    /// <param name="value">The value produced. Must not be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public static Result<T> Success(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "A success needs a value. Use Maybe<T> when absence is legal.");
        }

        return new Result<T>(value);
    }

    /// <summary>A failure described by <paramref name="error"/>.</summary>
    /// <param name="error">The error. Must be neither null nor <see cref="Error.None"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="error"/> is <see cref="Error.None"/>.</exception>
    public new static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        if (error == Error.None)
        {
            throw new ArgumentException("A failure needs a meaningful error, not Error.None.", nameof(error));
        }

        return new Result<T>(error);
    }

    /// <summary>Projects the value of a success; propagates the error of a failure.</summary>
    /// <typeparam name="TOut">The projected value's type.</typeparam>
    /// <param name="map">The projection, run only on a success.</param>
    public Result<TOut> Map<TOut>(Func<T, TOut> map) =>
        IsSuccess ? Result<TOut>.Success(map(Value)) : Result<TOut>.Failure(Error);

    /// <summary>Chains an operation that may itself fail.</summary>
    /// <typeparam name="TOut">The chained operation's value type.</typeparam>
    /// <param name="bind">The continuation, run only on a success.</param>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind) =>
        IsSuccess ? bind(Value) : Result<TOut>.Failure(Error);

    /// <summary>Lets a method return a bare value where a success is expected.</summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>Lets a method return an <see cref="BasicResults.Error"/> where a failure is expected.</summary>
    /// <param name="error">The error to wrap.</param>
    public static implicit operator Result<T>(Error error) => Failure(error);
}
