using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SampleApi.IntegrationTests;

public class HealthCheckTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/health", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HelloEndpoint_WithoutName_ReturnsDefaultMessage()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/hello", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Hello World!", content);
    }

    [Fact]
    public async Task HelloEndpoint_WithName_ReturnsPersonalizedMessage()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/hello?name=TestUser", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("TestUser", content);
    }
}
