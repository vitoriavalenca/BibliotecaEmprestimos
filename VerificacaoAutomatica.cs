namespace Biblioteca;

/// <summary>
/// Verificação automática simples, sem bibliotecas de teste.
/// Executada com: dotnet run -- teste
/// </summary>
public static class VerificacaoAutomatica
{
    private static int _falhas;

    public static int Executar()
    {
        _falhas = 0;
        var retirada = new DateOnly(2027, 3, 10);
        Console.WriteLine("=== Verificação automática ===");

        // 1. As duas políticas com a mesma data e o mesmo livro
        var aluno = new Emprestimo("Dom Casmurro", retirada, new PrazoAluno());
        var docente = new Emprestimo("Dom Casmurro", retirada, new PrazoDocente());
        Verificar("Aluno: devolução em 17/03/2027", aluno.CalcularDevolucao() == new DateOnly(2027, 3, 17));
        Verificar("Docente: devolução em 24/03/2027", docente.CalcularDevolucao() == new DateOnly(2027, 3, 24));
        Verificar("Mesmo livro e mesma retirada, prazos diferentes",
            aluno.Titulo == docente.Titulo
            && aluno.DataRetirada == docente.DataRetirada
            && aluno.CalcularDevolucao() != docente.CalcularDevolucao());

        // 2. Validações do construtor
        Verificar("Título vazio é rejeitado",
            LancaExcecao<ArgumentException>(() => new Emprestimo("", retirada, new PrazoAluno())));
        Verificar("Título só com espaços é rejeitado",
            LancaExcecao<ArgumentException>(() => new Emprestimo("   ", retirada, new PrazoAluno())));
        Verificar("Política nula é rejeitada",
            LancaExcecao<ArgumentNullException>(() => new Emprestimo("Dom Casmurro", retirada, null!)));

        // 3. Delegação com o objeto substituto
        var dataFixa = new DateOnly(2099, 12, 31);
        var substituto = new PrazoFixoTeste(dataFixa);
        var emprestimoTeste = new Emprestimo("Dom Casmurro", retirada, substituto);
        DateOnly devolucao = emprestimoTeste.CalcularDevolucao();

        Verificar("Substituto recebeu a data de retirada 10/03/2027", substituto.DataRecebida == retirada);
        Verificar("Substituto foi chamado exatamente 1 vez", substituto.QuantidadeChamadas == 1);
        Verificar("Emprestimo devolveu a data do substituto (31/12/2099)", devolucao == dataFixa);

        Console.WriteLine();
        Console.WriteLine(_falhas == 0
            ? "Resultado: todas as verificações passaram."
            : $"Resultado: {_falhas} verificação(ões) falharam.");

        return _falhas == 0 ? 0 : 1;
    }

    private static void Verificar(string descricao, bool condicao)
    {
        Console.WriteLine($"[{(condicao ? "OK" : "FALHOU")}] {descricao}");
        if (!condicao) _falhas++;
    }

    private static bool LancaExcecao<TExcecao>(Action acao) where TExcecao : Exception
    {
        try
        {
            acao();
            return false;
        }
        catch (TExcecao ex)
        {
            Console.WriteLine($"      mensagem: {ex.Message}");
            return true;
        }
    }
}
