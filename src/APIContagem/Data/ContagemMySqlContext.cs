using Microsoft.EntityFrameworkCore;

namespace APIContagem.Data;

public class ContagemMySqlContext : DbContext
{
    public DbSet<HistoricoContagemRegressiva>? Historicos { get; set; }

    public ContagemMySqlContext(DbContextOptions<ContagemMySqlContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistoricoContagemRegressiva>(entity =>
        {
            entity.ToTable("HistoricoContagem");
            entity.HasKey(c => c.Id);
        });
    }
}