var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health");

app.MapGet("/api/hello", (string? name) => 
{
    var greeting = string.IsNullOrEmpty(name) ? "Hello World!" : $"Hello {name}!";
    return Results.Ok(new { message = greeting });
})
    .WithName("Hello");

app.Run();

public partial class Program { }
