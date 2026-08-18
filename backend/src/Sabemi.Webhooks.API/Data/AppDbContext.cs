using Microsoft.EntityFrameworkCore;
using Sabemi.Webhooks.API.Models;

namespace Sabemi.Webhooks.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<LogEventoBruto> LogEventosBrutos { get; set; }
    public DbSet<StatusContrato> StatusContratos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEventoBruto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IdTransacao).IsUnique();
            entity.Property(e => e.Valor).HasPrecision(18, 2);
            entity.ToTable("log_eventos_brutos");
        });

        modelBuilder.Entity<StatusContrato>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IdContrato).IsUnique();
            entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            entity.ToTable("status_contratos");
        });
    }
}
