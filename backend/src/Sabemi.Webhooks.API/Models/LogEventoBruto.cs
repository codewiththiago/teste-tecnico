namespace Sabemi.Webhooks.API.Models;

public class LogEventoBruto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IdTransacao { get; set; } = string.Empty;
    public string IdContrato { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PayloadRaw { get; set; } = string.Empty;
    public DateTime RecebidoEm { get; set; } = DateTime.UtcNow;
    public bool Processado { get; set; } = false;
    public string? Erro { get; set; }
    public DateTime? ProcessadoEm { get; set; }
}
