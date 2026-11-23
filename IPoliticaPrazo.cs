namespace Biblioteca;

/// <summary>
/// Contrato do comportamento que varia: a política de prazo de devolução.
/// Emprestimo conhece apenas esta interface, nunca as classes concretas.
/// </summary>
public interface IPoliticaPrazo
{
    string Nome { get; }

    DateOnly CalcularDevolucao(DateOnly dataRetirada);
}
