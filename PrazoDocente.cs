namespace Biblioteca;

/// <summary>
/// Política Docente: devolução 14 dias corridos após a retirada.
/// </summary>
public class PrazoDocente : IPoliticaPrazo
{
    private const int DiasCorridos = 14;

    public string Nome => "Docente (14 dias)";

    public DateOnly CalcularDevolucao(DateOnly dataRetirada) => dataRetirada.AddDays(DiasCorridos);
}
