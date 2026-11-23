namespace Biblioteca;

/// <summary>
/// Política Aluno: devolução 7 dias corridos após a retirada.
/// </summary>
public class PrazoAluno : IPoliticaPrazo
{
    private const int DiasCorridos = 7;

    public string Nome => "Aluno (7 dias)";

    public DateOnly CalcularDevolucao(DateOnly dataRetirada) => dataRetirada.AddDays(DiasCorridos);
}
