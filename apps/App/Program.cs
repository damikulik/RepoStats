using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;

using RepoStats.AppHost.Module;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opts.AddConcurrencyLimiter("basic", o => o.PermitLimit = 50)
        .AddFixedWindowLimiter("fixed", o =>
        {
            o.PermitLimit = 100;
            o.Window = TimeSpan.FromSeconds(60);
        });
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddRepoStats(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapRepoStatsEndpoints().WithOpenApi();

await app.RunAsync();
