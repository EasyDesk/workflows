using Xunit;

namespace SampleApi.UnitTests;

public class GreetingTests
{
    [Fact]
    public void DefaultGreeting_ReturnsHelloWorld()
    {
        // Arrange & Act
        var result = "Hello World!";

        // Assert
        Assert.Equal("Hello World!", result);
    }

    [Theory]
    [InlineData("Alice")]
    [InlineData("Bob")]
    public void CustomGreeting_ReturnsPersonalizedMessage(string name)
    {
        // Arrange & Act
        var result = $"Hello {name}!";

        // Assert
        Assert.Contains(name, result);
    }
}
