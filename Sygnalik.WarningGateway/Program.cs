using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/warning", (Warning warning, IMemoryCache cache) =>
{
    cache.Set($"warning", warning, TimeSpan.FromMinutes(10));

    return Results.Ok(new
    {
        Message = "Warning added",
        Warning = warning
    });
});

app.MapGet("/warning", ([AsParameters] Location location, IMemoryCache cache) =>
{
    cache.Set($"location", location, TimeSpan.FromMinutes(10));

    if (cache.TryGetValue($"warning", out Warning? warning))
    {
        return Results.Ok(warning);
    }

    return Results.NotFound();
});

app.MapGet("/location", (IMemoryCache cache) =>
{
    if (cache.TryGetValue($"location", out Location? location))
    {
        return Results.Ok(location);
    }

    return Results.NotFound();
});

app.Run();

public class Warning
{
    public string Type { get; set; } = default!;
    public Location Location { get; set; } = default!;
}

public class Location
{
    public double Latitude { get; set; } = default!;
    public double Longitude { get; set; } = default!;
}