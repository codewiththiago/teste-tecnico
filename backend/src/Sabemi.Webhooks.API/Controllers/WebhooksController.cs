using Microsoft.AspNetCore.Mvc;
using Sabemi.Webhooks.API.DTOs;
using Sabemi.Webhooks.API.Filters;
using Sabemi.Webhooks.API.Services;
using System.Threading.Channels;

namespace Sabemi.Webhooks.API.Controllers;

[ApiController]
[Route("webhooks")]
[ServiceFilter(typeof(ApiKeyAuthFilter))]
public class WebhooksController : ControllerBase
{
    private readonly IWebhookService _webhookService;
    private readonly ChannelWriter<Guid> _channelWriter;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(
        IWebhookService webhookService,
        ChannelWriter<Guid> channelWriter,
        ILogger<WebhooksController> logger)
    {
        _webhookService = webhookService;
        _channelWriter = channelWriter;
        _logger = logger;
    }

    /// <summary>
    /// Recebe notificação de pagamento do banco parceiro.
    /// Requer header: X-Api-Key
    /// </summary>
    [HttpPost("pagamento")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReceberPagamento([FromBody] PagamentoWebhookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (isNew, eventId) = await _webhookService.ReceiveWebhookAsync(request);

        if (!isNew)
        {
            return Ok(new
            {
                message = "Evento já recebido anteriormente — idempotência garantida",
                id_transacao = request.IdTransacao
            });
        }

        await _channelWriter.WriteAsync(eventId);

        _logger.LogInformation("Webhook enfileirado para processamento: {IdTransacao}", request.IdTransacao);

        return Accepted(new
        {
            message = "Notificação recebida e enfileirada para processamento",
            id_transacao = request.IdTransacao,
            event_id = eventId
        });
    }
}
