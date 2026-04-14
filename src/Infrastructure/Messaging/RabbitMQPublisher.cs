using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PortfolioFila.Application.Interfaces;
using RabbitMQ.Client;

namespace PortfolioFila.Infrastructure.Messaging;

public class RabbitMQPublisher : IMessagePublisher
{
    private readonly RabbitMQConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQPublisher> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RabbitMQPublisher(RabbitMQConnectionFactory connectionFactory, ILogger<RabbitMQPublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message, JsonOptions);
        var body = Encoding.UTF8.GetBytes(json);

        using var channel = _connectionFactory.GetConnection().CreateModel();

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.MessageId = Guid.NewGuid().ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        channel.BasicPublish(
            exchange: $"{queueName}.exchange",
            routingKey: queueName,
            basicProperties: properties,
            body: body);

        _logger.LogInformation(
            "Message published to exchange {Exchange} | MessageId: {MessageId} | Size: {Size} bytes",
            $"{queueName}.exchange",
            properties.MessageId,
            body.Length);

        return Task.CompletedTask;
    }
}
