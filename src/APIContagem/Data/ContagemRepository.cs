using APIContagem.Models;

namespace APIContagem.Data;

public class ContagemRepository
{
    private readonly ContagemPostgresContext _context;

    public ContagemRepository(ContagemPostgresContext context)
    {
        _context = context;
    }

    public void Insert(ResultadoContador resultado)
    {
        _context.Historicos!.Add(new()
        {
            DataProcessamento = NodaTime.SystemClock.Instance.GetCurrentInstant(),
            ValorAtual = resultado.ValorAtual,
            Producer = resultado.Local,
            Kernel = resultado.Kernel,
            Framework = resultado.Framework,
            Mensagem = resultado.Mensagem
        });
        _context.SaveChanges();
    }
}