using GameWeb.Application;
using GameWeb.Infrastructure;
using GameWeb.Infrastructure.Data;
using GameWeb.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

// Use centralized exception handling registered via IExceptionHandler
app.UseExceptionHandler();

// CORS
app.UseCors();

// Authentication/Authorization for protected endpoints
app.UseAuthentication();
app.UseAuthorization();

app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.Run();

namespace GameWeb.Web
{
    public partial class Program { }
}
