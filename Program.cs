using System.Globalization;
using Biblioteca;

// Ponto de montagem (composition root): é o ÚNICO lugar que conhece
// as políticas concretas e decide qual será usada.

CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

if (args.Length == 0)
{
    MostrarUso();
    return 1;
}

string opcao = args[0].Trim().ToLowerInvariant();

if (opcao == "teste")
{
    return VerificacaoAutomatica.Executar();
}

IPoliticaPrazo? politica = opcao switch
{
    "aluno" => new PrazoAluno(),
    "docente" => new PrazoDocente(),
    _ => null
};

if (politica is null)
{
    Console.WriteLine($"Opção desconhecida: \"{args[0]}\". Nenhum empréstimo foi criado.");
    MostrarUso();
    return 1;
}

// Injeção de dependência pelo construtor
var emprestimo = new Emprestimo("Dom Casmurro", new DateOnly(2027, 3, 10), politica);

Console.WriteLine("=== Empréstimo ===");
Console.WriteLine($"Livro    : {emprestimo.Titulo}");
Console.WriteLine($"Política : {emprestimo.NomePolitica}");
Console.WriteLine($"Retirada : {emprestimo.DataRetirada:dd/MM/yyyy}");
Console.WriteLine($"Devolução: {emprestimo.CalcularDevolucao():dd/MM/yyyy}");
return 0;

static void MostrarUso()
{
    Console.WriteLine("Uso:");
    Console.WriteLine("  dotnet run -- aluno     (devolução em 7 dias corridos)");
    Console.WriteLine("  dotnet run -- docente   (devolução em 14 dias corridos)");
    Console.WriteLine("  dotnet run -- teste     (verificação automática)");
}
