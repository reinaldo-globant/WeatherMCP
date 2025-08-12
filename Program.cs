using System.Reflection;
using WeatherMCP.Application.Interfaces;
using WeatherMCP.Application.Services;
using WeatherMCP.Domain.Models;
using WeatherMCP.Infrastructure.ExternalServices;
using WeatherMCP.Infrastructure.MCP;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Services.Configure<MeteoChileOptions>(builder.Configuration.GetSection(MeteoChileOptions.SectionName));
builder.Services.Configure<McpServerOptions>(builder.Configuration.GetSection(McpServerOptions.SectionName));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WeatherMCP API",
        Version = "v1",
        Description = "API para acceso a datos meteorológicos de MeteoChile. Disponible como servidor MCP y como API REST.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "WeatherMCP",
            Url = new Uri("https://github.com/example/weathermcp")
        }
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddHttpClient<IMeteoChileApiClient, MeteoChileApiClient>();
builder.Services.AddScoped<IMeteoChileApiClient, MeteoChileApiClient>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<McpServer>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherMCP API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "WeatherMCP API Documentation";
    });
}

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithSummary("Verificación de salud del servidor")
   .WithDescription("Endpoint para verificar que el servidor esté funcionando correctamente");

app.MapControllers();

// Start MCP Server in background
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var mcpServer = scope.ServiceProvider.GetRequiredService<McpServer>();
    await mcpServer.RunAsync();
});

app.Run();
