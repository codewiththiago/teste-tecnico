using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sabemi.Webhooks.API.Data;

namespace Sabemi.Webhooks.API.Controllers;

[ApiController]
[Route("api")]
public class PagamentosController : ControllerBase
{
    private readonly AppDbContext _db;

    public PagamentosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("pagamentos")]
    public async Task<IActionResult> GetPagamentos(
        [FromQuery] string? status,
        [FromQuery] string? id_contrato)
    {
        var query = _db.LogEventosBrutos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status.ToLower() == status.ToLower());

        if (!string.IsNullOrWhiteSpace(id_contrato))
            query = query.Where(e => e.IdContrato.Contains(id_contrato));

        var pagamentos = await query
            .OrderByDescending(e => e.RecebidoEm)
            .Select(e => new
            {
                e.Id,
                e.IdTransacao,
                e.IdContrato,
                e.Valor,
                e.DataPagamento,
                e.Status,
                e.RecebidoEm,
                e.Processado,
                e.Erro,
                e.ProcessadoEm,
                TemErro = e.Erro != null
            })
            .ToListAsync();

        return Ok(pagamentos);
    }

    [HttpGet("contratos")]
    public async Task<IActionResult> GetContratos()
    {
        var contratos = await _db.StatusContratos
            .AsNoTracking()
            .OrderByDescending(c => c.UltimaAtualizacao)
            .ToListAsync();

        return Ok(contratos);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.LogEventosBrutos.CountAsync();
        var comErro = await _db.LogEventosBrutos.CountAsync(e => e.Erro != null);
        var processados = await _db.LogEventosBrutos.CountAsync(e => e.Processado);
        var pendentes = total - processados;

        return Ok(new { total, comErro, processados, pendentes });
    }
}
