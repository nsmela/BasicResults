namespace BasicResults;

/// <summary>
/// A strongly-typed domain error. Errors are values, not exceptions: they are
/// compared, returned and pattern-matched like any other value object.
/// </summary>
/// <param name="Code">A stable, machine-readable identifier, conventionally
/// <c>Area.Reason</c> — callers compare against this rather than the description.</param>
/// <param name="Description">A human-readable explanation, for logs and messages.</param>
public sealed record Error(string Code, string Description)
{
    /// <summary>Sentinel for "no error". Only ever valid on a success result.</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Renders the error as <c>Code: Description</c>.</summary>
    public override string ToString() => $"{Code}: {Description}";
}
