# Biblioteca — Empréstimos e políticas de prazo

Atividade 02 — Programação Orientada a Objetos (Guias 01–03).
Aplicação de console em C# (.NET 10, `net10.0`) que calcula a data de devolução de
um empréstimo usando composição, delegação, interface, polimorfismo e injeção de
dependência.

## Estrutura

| Arquivo | Papel |
|---|---|
| `IPoliticaPrazo.cs` | Interface (contrato) do comportamento que varia |
| `PrazoAluno.cs` | Política Aluno: devolução 7 dias corridos após a retirada |
| `PrazoDocente.cs` | Política Docente: devolução 14 dias corridos após a retirada |
| `PrazoFixoTeste.cs` | Objeto substituto: devolve uma data fixa e registra a data recebida |
| `Emprestimo.cs` | Classe principal: dados do domínio + delegação do prazo |
| `VerificacaoAutomatica.cs` | Testes simples executados pelo terminal |
| `Program.cs` | Ponto de montagem: escolhe a política e injeta no empréstimo |

## Como executar

Na pasta do projeto:

```bash
dotnet run -- aluno
dotnet run -- docente
dotnet run -- teste
```

Dados usados no exemplo: livro "Dom Casmurro", retirada em 10/03/2027.

## Exemplo 1 — Política Aluno

```
$ dotnet run -- aluno
=== Empréstimo ===
Livro    : Dom Casmurro
Política : Aluno (7 dias)
Retirada : 10/03/2027
Devolução: 17/03/2027
```

10/03/2027 + 7 dias corridos = **17/03/2027**

## Exemplo 2 — Política Docente

```
$ dotnet run -- docente
=== Empréstimo ===
Livro    : Dom Casmurro
Política : Docente (14 dias)
Retirada : 10/03/2027
Devolução: 24/03/2027
```

10/03/2027 + 14 dias corridos = **24/03/2027**

**Comparação:** mesmo livro e mesma data de retirada. Só a política muda, e a
devolução muda junto (17/03 contra 24/03). `Emprestimo` é o mesmo nos dois casos.

## Opção inválida

```
$ dotnet run -- bibliotecario
Opção desconhecida: "bibliotecario". Nenhum empréstimo foi criado.
Uso:
  dotnet run -- aluno     (devolução em 7 dias corridos)
  dotnet run -- docente   (devolução em 14 dias corridos)
  dotnet run -- teste     (verificação automática)
```

O `Program.cs` verifica a opção **antes** de criar qualquer objeto. Se ela não
existe, mostra o uso e termina com código 1, sem criar um `Emprestimo`.

## Verificação automática (objeto de teste)

```
$ dotnet run -- teste
=== Verificação automática ===
[OK] Aluno: devolução em 17/03/2027
[OK] Docente: devolução em 24/03/2027
[OK] Mesmo livro e mesma retirada, prazos diferentes
      mensagem: O título do livro é obrigatório e não pode ficar vazio. (Parameter 'titulo')
[OK] Título vazio é rejeitado
      mensagem: O título do livro é obrigatório e não pode ficar vazio. (Parameter 'titulo')
[OK] Título só com espaços é rejeitado
      mensagem: A política de prazo é obrigatória. (Parameter 'politica')
[OK] Política nula é rejeitada
[OK] Substituto recebeu a data de retirada 10/03/2027
[OK] Substituto foi chamado exatamente 1 vez
[OK] Emprestimo devolveu a data do substituto (31/12/2099)

Resultado: todas as verificações passaram.
```

O `PrazoFixoTeste` devolve sempre 31/12/2099, uma data que nenhuma política real
produziria a partir de 10/03/2027. Se `Emprestimo.CalcularDevolucao()` retorna
31/12/2099, fica provado que o empréstimo usou o retorno do colaborador. A data
registrada pelo substituto (10/03/2027) prova que o empréstimo repassou a data de
retirada correta.

### Mensagens das validações do construtor

- **Título vazio ou só com espaços** → `ArgumentException`:
  *"O título do livro é obrigatório e não pode ficar vazio. (Parameter 'titulo')"*.
  Um empréstimo sem livro não tem sentido, então o construtor recusa o objeto. O
  `(Parameter 'titulo')` é acrescentado pelo .NET e indica qual argumento estava errado.
- **Política nula** → `ArgumentNullException`:
  *"A política de prazo é obrigatória. (Parameter 'politica')"*.
  Sem política, `Emprestimo` não teria a quem delegar o cálculo. A falha acontece
  já na criação, e não mais tarde, quando alguém pedisse a devolução.

Nos dois casos o objeto nunca chega a existir, então não sobra nenhum empréstimo
inconsistente no programa.

## Justificativas

**Composição.** `Emprestimo` *tem uma* `IPoliticaPrazo` (campo `_politica`). O prazo
não é herdado: ele vem de um objeto separado que sabe calcular a devolução.

**Interface.** `IPoliticaPrazo` define só o contrato (`CalcularDevolucao` e `Nome`).
Não usei classe abstrata porque as políticas não compartilham estado nem código.
Cada uma tem apenas o próprio número de dias.

**Injeção de dependência.** A política chega pelo construtor de `Emprestimo`, que
valida se ela não é nula. Quem escolhe a classe concreta é o `Program.cs`, sem
nenhuma biblioteca de DI.

**Polimorfismo.** `Emprestimo` chama `_politica.CalcularDevolucao(...)` sem saber
qual classe está por trás. A implementação certa é executada sem nenhuma
verificação de tipo.

## Questões

**1. Por que o prazo é um comportamento que pode variar independentemente do empréstimo?**
Porque o empréstimo é sempre o mesmo: um livro retirado em uma data. O que muda é
a regra da biblioteca para quem pegou o livro, e essa regra pode mudar sem que o
empréstimo mude (a biblioteca pode passar o prazo de aluno para 10 dias, ou criar
um público novo). Por isso o prazo fica num objeto separado, que pode ser trocado
sem mexer em `Emprestimo`.

**2. Qual é a diferença entre a relação "é um" e a relação "tem um" nesse exemplo?**
- **"É um" (herança/implementação):** `PrazoAluno` *é uma* `IPoliticaPrazo`, e
  `PrazoDocente` também. Por isso as duas podem ser usadas no lugar do contrato.
- **"Tem um" (composição):** `Emprestimo` *tem uma* `IPoliticaPrazo`. Ele não é uma
  política: ele guarda uma e pede a ela o cálculo.

Se usássemos "é um" para o empréstimo (`EmprestimoAluno : Emprestimo`), cada tipo
de público viraria uma subclasse e o prazo ficaria preso ao objeto para sempre.

**3. Por que a classe `Emprestimo` não deveria instanciar `PrazoAluno` diretamente?**
Se `Emprestimo` fizesse `new PrazoAluno()`, ficaria amarrado a essa classe
concreta: todo empréstimo seria de aluno, e trocar a política exigiria alterar
`Emprestimo`. Também não daria para testá-lo com o `PrazoFixoTeste`. Recebendo a
política pronta pelo construtor, `Emprestimo` depende só da interface, e a escolha
fica no `Program.cs`.

**4. Como uma política Pesquisador, de 21 dias corridos, poderia ser adicionada sem alterar `Emprestimo`?**
- Criar `PrazoPesquisador.cs`, implementando `IPoliticaPrazo` com
  `dataRetirada.AddDays(21)`;
- Adicionar a opção `"pesquisador" => new PrazoPesquisador()` no `Program.cs`
  (e a linha correspondente em `MostrarUso`).

`Emprestimo.cs` e `IPoliticaPrazo.cs` não mudam.

## Restrição de arquitetura

`Emprestimo.CalcularDevolucao()` não possui `if` nem `switch` para consultar o
perfil. O único `if` da classe fica no construtor e serve para validar o título,
não para escolher política. A escolha da política acontece só no `Program.cs`, o
ponto de montagem.
