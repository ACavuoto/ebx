using Ebx.Test.WebApi.Extensions;
using Ebx.Test.WebApi.Validators.V1;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddOctokitGitHubClient(builder.Configuration)
    .AddValidatorsFromAssemblyContaining<StringValidator>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();

/// <summary>
/// Since .NET 8 has removed the startup.cs, program.cs has become internal.
/// In order to enable usage as a marker in WebApplicationFactory this partial declaration is added as per the Microsoft docs:
/// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
/// </summary>
public partial class Program
{ }