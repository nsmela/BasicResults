using Xunit;

namespace BasicResults.Tests;

public class ErrorTests
{
    [Fact]
    public void None_IsTheEmptyCodeAndDescription()
    {
        Assert.Equal(string.Empty, Error.None.Code);
        Assert.Equal(string.Empty, Error.None.Description);
    }

    [Fact]
    public void Errors_AreComparedByValueNotReference()
    {
        var first = new Error("Mesh.Empty", "The mesh has no triangles.");
        var second = new Error("Mesh.Empty", "The mesh has no triangles.");

        Assert.Equal(first, second);
        Assert.NotSame(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Errors_WithDifferentCodes_AreNotEqual()
    {
        var empty = new Error("Mesh.Empty", "Same description.");
        var open = new Error("Mesh.Open", "Same description.");

        Assert.NotEqual(empty, open);
    }

    [Fact]
    public void ToString_ReadsAsCodeThenDescription()
    {
        var error = new Error("Mesh.Empty", "The mesh has no triangles.");

        Assert.Equal("Mesh.Empty: The mesh has no triangles.", error.ToString());
    }
}
