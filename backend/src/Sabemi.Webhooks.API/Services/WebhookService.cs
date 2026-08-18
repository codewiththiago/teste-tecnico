using Microsoft.EntityFrameworkCore;
using Sabemi.Webhooks.API.Data;
using Sabemi.Webhooks.API.DTOs;
using Sabemi.Webhooks.API.Models;
using System.Text.Json;

namespace Sabemi.Webhooks.API.Services;

public class WebhookService : IWebhookService
{
    private readonly AppDbContext _db;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(AppDbContext db, ILogger<WebhookService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<(bool IsNew, Guid EventId)> ReceiveWebhookAsync(PagamentoWebhookRequest request)
    {
        var existing = await _db.LogEventosBrutos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdTransacao == request.IdTransacao);

        if (existing != null)
        {
            _logger.LogWarning("Transação duplicada ignorada: {IdTransacao}", request.IdTransacao);
            return (false, existing.Id);
        }

        var evento = new LogEventoBruto
        {
            IdTransacao = request.IdTransacao,
            IdContrato = request.IdContrato,
            Valor = request.Valor,
            DataPagamento = request.DataPagamento,
            Status = request.Status,
            PayloadRaw = JsonSerializer.Serialize(request)
        };

        _db.LogEventosBrutos.Add(evento);

        try
        {
            await _db.SaveChangesAsync();
            return (true, evento.Id);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            // Race condition: another request saved the same id_transacao concurrently
            _logger.LogWarning("Conflito de idempotência detectado para: {IdTransacao}", request.IdTransacao);
            var conflict = await _db.LogEventosBrutos
                .AsNoTracking()
                .FirstAsync(e => e.IdTransacao == request.IdTransacao);
            return (false, conflict.Id);
        }
    }

    public async Task ProcessWebhookAsync(Guid eventId)
    {
        // Simulate heavy business rule processing
        await Task.Delay(TimeSpan.FromSeconds(2));

        var evento = await _db.LogEventosBrutos.FindAsync(eventId);
        if (evento == null) return;

        try
        {
            ValidateBusinessRules(evento);

            var contrato = await _db.StatusContratos
                .FirstOrDefaultAsync(c => c.IdContrato == evento.IdContrato);

            if (contrato == null)
            {
                contrato = new StatusContrato
                {
                    IdContrato = evento.IdContrato,
                    UltimoPagamentoId = evento.IdTransacao,
                    ValorTotal = evento.Valor,
                    Status = evento.Status,
                    TotalPagamentos = 1
                };
                _db.StatusContratos.Add(contrato);
            }
            else
            {
                contrato.UltimoPagamentoId = evento.IdTransacao;
                contrato.ValorTotal += evento.Valor;
                contrato.Status = evento.Status;
                contrato.UltimaAtualizacao = DateTime.UtcNow;
                contrato.TotalPagamentos++;
            }

            evento.Processado = true;
            evento.ProcessadoEm = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Evento {EventId} processado com sucesso", eventId);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            evento.Processado = true;
            evento.Erro = ex.Message;
            evento.ProcessadoEm = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            _logger.LogError(ex, "Erro ao processar evento {EventId}", eventId);
        }
    }

    private static void ValidateBusinessRules(LogEventoBruto evento)
    {
        if (evento.Valor <= 0)
            throw new InvalidOperationException("Valor do pagamento inválido (deve ser maior que zero)");

        if (string.IsNullOrWhiteSpace(evento.Status))
            throw new InvalidOperationException("Status do pagamento ausente ou inválido");

        var statusValidos = new[] { "sucesso", "erro", "pendente" };
        if (!statusValidos.Contains(evento.Status.ToLower()))
            throw new InvalidOperationException($"Status '{evento.Status}' não reconhecido");
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true
            || ex.InnerException?.Message.Contains("23505") == true
            || ex.InnerException?.Message.Contains("IX_") == true;
    }
}
