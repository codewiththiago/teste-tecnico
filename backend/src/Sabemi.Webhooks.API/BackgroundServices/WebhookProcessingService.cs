using Sabemi.Webhooks.API.Services;
using System.Threading.Channels;

namespace Sabemi.Webhooks.API.BackgroundServices;

public class WebhookProcessingService : BackgroundService
{
    private readonly ChannelReader<Guid> _reader;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WebhookProcessingService> _logger;

    public WebhookProcessingService(
        ChannelReader<Guid> reader,
        IServiceScopeFactory scopeFactory,
        ILogger<WebhookProcessingService> logger)
    {
        _reader = reader;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço de processamento de webhooks iniciado");

        await foreach (var eventId in _reader.ReadAllAsync(stoppingToken))
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IWebhookService>();
                    await service.ProcessWebhookAsync(eventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha crítica ao processar evento {EventId}", eventId);
                }
            }, stoppingToken);
        }
    }
}
