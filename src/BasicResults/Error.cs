namespace BasicResults;

/// <summary>
/// A strongly-typed domain error. Errors are values, not exceptions: they are
/// compared, returned and pattern-matched like any other value object.
/// </summary>
/// <remarks>
/// The properties are get-only and set through the constructor rather than declared
/// positionally. A positional record compiles them to init-only setters, and the marker type
/// those need - <c>System.Runtime.CompilerServices.IsExternalInit</c> - arrived in .NET 5,
/// after netstandard2.0. Writing them this way keeps the library free of a polyfill for it.
/// The cost is that <c>with</c> expressions and positional deconstruction are unavailable on
/// this type; value equality, <c>IEquatable&lt;Error&gt;</c> and <c>GetHashCode</c> are not
/// affected, as those come from <c>record</c> rather than from the setters.
/// </remarks>
public sealed record Error
{
    /// <summary>Creates an error.</summary>
    /// <param name="code">A stable, machine-readable identifier, conventionally
    /// <c>Area.Reason</c> - callers compare against this rather than the description.</param>
    /// <param name="description">A human-readable explanation, for logs and messages.</param>
    public Error(string code, string description)
    {
        Code = code;
        Description = description;
    }

    /// <summary>
    /// A stable, machine-readable identifier, conventionally <c>Area.Reason</c>. This is what
    /// callers compare against.
    /// </summary>
    public string Code { get; }

    /// <summary>A human-readable explanation, for logs and messages.</summary>
    public string Description { get; }

    /// <summary>Sentinel for "no error". Only ever valid on a success result.</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Renders the error as <c>Code: Description</c>.</summary>
    public override string ToString() => $"{Code}: {Description}";
}
