namespace Biblioteca;

/// <summary>
/// Objeto substituto para teste: devolve sempre a data configurada
/// e registra a data de retirada que recebeu. Não usa nenhuma regra real.
/// </summary>
public class PrazoFixoTeste : IPoliticaPrazo
{
    private readonly DateOnly _dataFixa;

    public PrazoFixoTeste(DateOnly dataFixa)
    {
        _dataFixa = dataFixa;
    }

    public string Nome => "Prazo fixo de teste";

    public DateOnly? DataRecebida { get; private set; }
    public int QuantidadeChamadas { get; private set; }

    public DateOnly CalcularDevolucao(DateOnly dataRetirada)
    {
        DataRecebida = dataRetirada;
        QuantidadeChamadas++;
        return _dataFixa;
    }
}
