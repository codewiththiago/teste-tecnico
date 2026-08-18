using Sabemi.Webhooks.API.DTOs;

namespace Sabemi.Webhooks.API.Services;

public interface IWebhookService
{
    Task<(bool IsNew, Guid EventId)> ReceiveWebhookAsync(PagamentoWebhookRequest request);
    Task ProcessWebhookAsync(Guid eventId);
}
