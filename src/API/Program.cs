using PortfolioFila.Application.Extensions;
using PortfolioFila.Infrastructure.Extensions;
using PortfolioFila.Infrastructure.Messaging;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog ───────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day));

// ─── Services ──────────────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Order Processing API",
        Version = "v1",
        Description = "REST API for order processing with asynchronous queue — built with .NET 8, DDD & Clean Architecture."
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var rabbitSettings = builder.Configuration.GetSection(RabbitMQSettings.SectionName);
builder.Services
    .AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sqlserver",
        tags: ["db", "sql"])
    .AddRabbitMQ(
        rabbitConnectionString: $"amqp://{rabbitSettings["Username"]}:{rabbitSettings["Password"]}@{rabbitSettings["Host"]}:{rabbitSettings["Port"]}{rabbitSettings["VirtualHost"]}",
        name: "rabbitmq",
        tags: ["mq", "rabbit"]);

// ─── App ───────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Processing API v1"));
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

// Ensure RabbitMQ queues exist on startup
using (var scope = app.Services.CreateScope())
{
    var queueSetup = scope.ServiceProvider.GetRequiredService<RabbitMQQueueSetup>();
    try
    {
        queueSetup.EnsureQueuesExist();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Could not setup RabbitMQ queues on startup. Make sure RabbitMQ is running.");
    }
}

app.Run();

public partial class Program { }
