using Microsoft.EntityFrameworkCore;
using Sabemi.Webhooks.API.BackgroundServices;
using Sabemi.Webhooks.API.Data;
using Sabemi.Webhooks.API.Filters;
using Sabemi.Webhooks.API.Services;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Sabemi Webhooks API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-Api-Key",
        Description = "API Key para autenticação do webhook"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "ApiKey" } },
            Array.Empty<string>()
        }
    });
});

// PostgreSQL via EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unbounded channel for background processing queue
var channel = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions { SingleReader = false });
builder.Services.AddSingleton(channel);
builder.Services.AddSingleton(channel.Writer);
builder.Services.AddSingleton(channel.Reader);

// Filters & services
builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddHostedService<WebhookProcessingService>();

// CORS — allows the Next.js dashboard to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var origins = builder.Configuration["AllowedOrigins"] ?? "http://localhost:3000";
        policy.WithOrigins(origins.Split(','))
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Create schema on startup (use EF migrations for production)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sabemi Webhooks API v1"));

app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
