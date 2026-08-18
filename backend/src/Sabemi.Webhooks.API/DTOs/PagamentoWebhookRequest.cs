using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sabemi.Webhooks.API.DTOs;

public class PagamentoWebhookRequest
{
    [Required]
    [JsonPropertyName("id_transacao")]
    public string IdTransacao { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("id_contrato")]
    public string IdContrato { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [Required]
    [JsonPropertyName("data_pagamento")]
    public DateTime DataPagamento { get; set; }

    [Required]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
