namespace Biblioteca;

/// <summary>
/// Classe principal do domínio. Guarda o título e a data de retirada e
/// DELEGA o cálculo da devolução à política recebida pelo construtor.
/// Não contém if/switch para distinguir o público e não instancia
/// nenhuma política concreta.
/// </summary>
public class Emprestimo
{
    private readonly IPoliticaPrazo _politica; // composição: Emprestimo "tem uma" política

    public Emprestimo(string titulo, DateOnly dataRetirada, IPoliticaPrazo politica)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("O título do livro é obrigatório e não pode ficar vazio.", nameof(titulo));
        }

        _politica = politica
            ?? throw new ArgumentNullException(nameof(politica), "A política de prazo é obrigatória.");

        Titulo = titulo.Trim();
        DataRetirada = dataRetirada;
    }

    public string Titulo { get; }
    public DateOnly DataRetirada { get; }

    public string NomePolitica => _politica.Nome;

    // Delegação: o empréstimo repassa a data de retirada à política e usa o retorno.
    public DateOnly CalcularDevolucao() => _politica.CalcularDevolucao(DataRetirada);
}
