
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = string.Format(
    "Host={0};Port={1};Database={2};Username={3};Password={4}",
    Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost",
    Environment.GetEnvironmentVariable("DB_PORT") ?? "5432",
    Environment.GetEnvironmentVariable("DB_NAME") ?? "andrezog",
    Environment.GetEnvironmentVariable("DB_USER") ?? "postgres",
    Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres"
);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
