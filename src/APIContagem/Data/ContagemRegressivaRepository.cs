using APIContagem.Models;

namespace APIContagem.Data;

public class ContagemRegressivaRepository
{
    private readonly ContagemMySqlContext _context;

    public ContagemRegressivaRepository(ContagemMySqlContext context)
    {
        _context = context;
    }

    public void Insert(ResultadoContador resultado)
    {
        _context.Historicos!.Add(new()
        {
            DataProcessamento = DateTime.Now,
            ValorAtual = resultado.ValorAtual,
            Producer = resultado.Local,
            Kernel = resultado.Kernel,
            Framework = resultado.Framework,
            Mensagem = resultado.Mensagem
        });
        _context.SaveChanges();
    }
}