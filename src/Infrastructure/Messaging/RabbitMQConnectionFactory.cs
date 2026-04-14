using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PortfolioFila.Infrastructure.Messaging;

public class RabbitMQConnectionFactory : IDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQConnectionFactory> _logger;
    private IConnection? _connection;
    private readonly object _lock = new();

    public RabbitMQConnectionFactory(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQConnectionFactory> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public IConnection GetConnection()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        lock (_lock)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            _logger.LogInformation("Creating RabbitMQ connection to {Host}:{Port}", _settings.Host, _settings.Port);

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _logger.LogInformation("RabbitMQ connection established");
        }

        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { IsOpen: true })
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
