using APIContagem.Data;
using APIContagem.Models;
using APIContagem.Tracing;
using Microsoft.AspNetCore.Mvc;

namespace APIContagem.Controllers;

[ApiController]
[Route("[controller]")]
public class ContadorController : ControllerBase
{
    private readonly static Lock ContagemLock = new();

    private readonly ILogger<ContadorController> _logger;
    private readonly IConfiguration _configuration;
    private readonly Contador _contador;

    public ContadorController(ILogger<ContadorController> logger,
        IConfiguration configuration,
        Contador contador)
    {
        _logger = logger;
        _configuration = configuration;
        _contador = contador;
    }

    [HttpGet]
    public ResultadoContador Get(
        [FromServices]ContagemRepository repository)
    {
        using var activity1 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("GerarValorContagem")!;

        int valorAtualContador;
        using (ContagemLock.EnterScope())
        {
            _contador.Incrementar();
            valorAtualContador = _contador.ValorAtual;
        }
        activity1.SetTag("valorAtual", valorAtualContador);
        _logger.LogInformation($"Contador - Valor atual: {valorAtualContador}");

        var resultado = new ResultadoContador()
        {
            ValorAtual = valorAtualContador,
            Local = _contador.Local,
            Kernel = _contador.Kernel,
            Mensagem = _configuration["Saudacao"],
            Framework = _contador.Framework
        };
        activity1.Stop();

        using var activity2 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("RegistrarRetornarValorContagem")!;

        repository.Insert(resultado);
        _logger.LogInformation($"Registro inserido com sucesso! Valor: {valorAtualContador}");

        activity2.SetTag("valorAtual", valorAtualContador);
        activity2.SetTag("horario", $"{DateTime.UtcNow.AddHours(-3):HH:mm:ss}");

        return resultado;
    }

    [HttpGet("regressivo")]
    public ResultadoContador GetContagemRegressiva(
        [FromServices]ContagemRegressivaRepository repository)
    {
        using var activity1 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("GerarValorContagemRegressiva")!;

        int valorAtualContador;
        using (ContagemLock.EnterScope())
        {
            _contador.Decrementar();
            valorAtualContador = _contador.ValorAtualRegressivo;
        }
        activity1.SetTag("valorAtualRegressivo", valorAtualContador);
        _logger.LogInformation($"Contador Regressivo - Valor atual: {valorAtualContador}");

        var resultado = new ResultadoContador()
        {
            ValorAtual = valorAtualContador,
            Local = _contador.Local,
            Kernel = _contador.Kernel,
            Mensagem = _configuration["Saudacao"],
            Framework = _contador.Framework
        };
        activity1.Stop();

        using var activity2 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("RegistrarRetornarValorContagemRegressiva")!;

        repository.Insert(resultado);
        _logger.LogInformation($"Registro inserido com sucesso! Valor regressivo: {valorAtualContador}");

        activity2.SetTag("valorAtualRegressivo", valorAtualContador);
        activity2.SetTag("horario", $"{DateTime.UtcNow.AddHours(-3):HH:mm:ss}");

        return resultado;
    }
}