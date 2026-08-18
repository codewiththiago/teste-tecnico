namespace Sabemi.Webhooks.API.Models;

public class StatusContrato
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IdContrato { get; set; } = string.Empty;
    public string UltimoPagamentoId { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;
    public int TotalPagamentos { get; set; }
}
