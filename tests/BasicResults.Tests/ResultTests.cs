using Xunit;

namespace BasicResults.Tests;

public class ResultTests
{
    private static readonly Error Failed = new("Test.Failed", "The operation failed.");

    [Fact]
    public void Success_IsSuccessAndNotFailure()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Failure_CarriesTheError()
    {
        var result = Result.Failure(Failed);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(Failed, result.Error);
    }

    [Fact]
    public void Error_OnASuccess_Throws()
    {
        var result = Result.Success();

        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Failure_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
    }

    [Fact]
    public void Failure_WithErrorNone_Throws()
    {
        // The sentinel means "no error"; a failure carrying it would report success by another name.
        Assert.Throws<ArgumentException>(() => Result.Failure(Error.None));
    }

    [Fact]
    public void Generic_Success_CarriesTheValue()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Generic_Value_OnAFailure_ThrowsNamingTheError()
    {
        var result = Result.Failure<int>(Failed);

        var thrown = Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Contains("Test.Failed", thrown.Message);
    }

    [Fact]
    public void Generic_Success_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Result.Success<string>(null!));
    }

    [Fact]
    public void Generic_Failure_WithErrorNone_Throws()
    {
        Assert.Throws<ArgumentException>(() => Result<int>.Failure(Error.None));
    }

    [Fact]
    public void AnError_ConvertsImplicitlyToAFailure()
    {
        Result untyped = Failed;
        Result<int> typed = Failed;

        Assert.True(untyped.IsFailure);
        Assert.True(typed.IsFailure);
        Assert.Equal(Failed, typed.Error);
    }

    [Fact]
    public void AValue_ConvertsImplicitlyToASuccess()
    {
        Result<int> result = 7;

        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Value);
    }

    [Fact]
    public void Map_ProjectsASuccess()
    {
        var mapped = Result.Success(21).Map(x => x * 2);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(42, mapped.Value);
    }

    [Fact]
    public void Map_PropagatesAFailureWithoutRunningTheProjection()
    {
        var ran = false;

        var mapped = Result.Failure<int>(Failed).Map(x =>
        {
            ran = true;
            return x * 2;
        });

        Assert.False(ran);
        Assert.True(mapped.IsFailure);
        Assert.Equal(Failed, mapped.Error);
    }

    [Fact]
    public void Bind_ChainsASuccess()
    {
        var bound = Result.Success(21).Bind(x => Result.Success(x.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("21", bound.Value);
    }

    [Fact]
    public void Bind_SurfacesTheFailureOfTheChainedOperation()
    {
        var bound = Result.Success(21).Bind(_ => Result.Failure<string>(Failed));

        Assert.True(bound.IsFailure);
        Assert.Equal(Failed, bound.Error);
    }

    [Fact]
    public void Bind_PropagatesAFailureWithoutRunningTheContinuation()
    {
        var ran = false;

        var bound = Result.Failure<int>(Failed).Bind(x =>
        {
            ran = true;
            return Result.Success(x.ToString());
        });

        Assert.False(ran);
        Assert.True(bound.IsFailure);
        Assert.Equal(Failed, bound.Error);
    }
}
