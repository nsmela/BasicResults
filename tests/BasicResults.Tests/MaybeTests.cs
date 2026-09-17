using Xunit;

namespace BasicResults.Tests;

public class MaybeTests
{
    private static readonly Error Missing = new("Test.Missing", "Nothing was there.");

    [Fact]
    public void Some_HoldsTheValue()
    {
        var maybe = Maybe<int>.Some(42);

        Assert.True(maybe.HasValue);
        Assert.False(maybe.HasNoValue);
        Assert.Equal(42, maybe.Value);
    }

    [Fact]
    public void None_HoldsNothing()
    {
        var maybe = Maybe<int>.None();

        Assert.False(maybe.HasValue);
        Assert.True(maybe.HasNoValue);
    }

    [Fact]
    public void TheDefaultInstance_IsNone()
    {
        // A struct can always be default-constructed, so default has to be the empty case.
        Maybe<string> defaulted = default;

        Assert.True(defaulted.HasNoValue);
        Assert.Equal(Maybe<string>.None(), defaulted);
    }

    [Fact]
    public void Some_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Maybe<string>.Some(null!));
    }

    [Fact]
    public void Value_OnNone_Throws()
    {
        var maybe = Maybe<int>.None();

        Assert.Throws<InvalidOperationException>(() => maybe.Value);
    }

    [Fact]
    public void GetValueOrDefault_FallsBackOnlyWhenEmpty()
    {
        Assert.Equal(42, Maybe<int>.Some(42).GetValueOrDefault(7));
        Assert.Equal(7, Maybe<int>.None().GetValueOrDefault(7));
    }

    [Fact]
    public void Map_ProjectsAValueAndStaysEmptyOtherwise()
    {
        Assert.Equal("42", Maybe<int>.Some(42).Map(x => x.ToString()).Value);
        Assert.True(Maybe<int>.None().Map(x => x.ToString()).HasNoValue);
    }

    [Fact]
    public void Map_OnNone_DoesNotRunTheProjection()
    {
        var ran = false;

        Maybe<int>.None().Map(x =>
        {
            ran = true;
            return x;
        });

        Assert.False(ran);
    }

    [Fact]
    public void Bind_ChainsAndCollapsesToNone()
    {
        Assert.Equal("42", Maybe<int>.Some(42).Bind(x => Maybe<string>.Some(x.ToString())).Value);
        Assert.True(Maybe<int>.Some(42).Bind(_ => Maybe<string>.None()).HasNoValue);
        Assert.True(Maybe<int>.None().Bind(x => Maybe<string>.Some(x.ToString())).HasNoValue);
    }

    [Fact]
    public void ToResult_TurnsAbsenceIntoTheGivenFailure()
    {
        var present = Maybe<int>.Some(42).ToResult(Missing);
        var absent = Maybe<int>.None().ToResult(Missing);

        Assert.True(present.IsSuccess);
        Assert.Equal(42, present.Value);
        Assert.True(absent.IsFailure);
        Assert.Equal(Missing, absent.Error);
    }

    [Fact]
    public void Equality_HoldsForEqualValuesAndForTwoEmpties()
    {
        Assert.Equal(Maybe<int>.Some(42), Maybe<int>.Some(42));
        Assert.Equal(Maybe<int>.None(), Maybe<int>.None());
        Assert.NotEqual(Maybe<int>.Some(42), Maybe<int>.Some(7));
        Assert.NotEqual(Maybe<int>.Some(42), Maybe<int>.None());
    }

    [Fact]
    public void Operators_AgreeWithEquals()
    {
        Assert.True(Maybe<int>.Some(42) == Maybe<int>.Some(42));
        Assert.True(Maybe<int>.Some(42) != Maybe<int>.Some(7));
        Assert.True(Maybe<int>.None() == Maybe<int>.None());
        Assert.False(Maybe<int>.None() != Maybe<int>.None());
    }

    [Fact]
    public void EqualValues_ShareAHashCode()
    {
        Assert.Equal(Maybe<int>.Some(42).GetHashCode(), Maybe<int>.Some(42).GetHashCode());
        Assert.Equal(0, Maybe<int>.None().GetHashCode());
    }

    [Fact]
    public void Equals_AgainstAnUnrelatedObject_IsFalse()
    {
        Assert.False(Maybe<int>.Some(42).Equals("42"));
    }

    [Fact]
    public void ToString_DistinguishesSomeFromNone()
    {
        Assert.Equal("Some(42)", Maybe<int>.Some(42).ToString());
        Assert.Equal("None", Maybe<int>.None().ToString());
    }
}
