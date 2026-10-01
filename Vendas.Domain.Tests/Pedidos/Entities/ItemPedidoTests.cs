using FluentAssertions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Entities;

namespace Vendas.Domain.Tests.Pedidos.Entities;

public class ItemPedidoTests
{
    //metodo auxiliar
    private static ItemPedido CriarItemValido(decimal preco = 100m, int quantidade = 2)
    {
        return new ItemPedido(Guid.NewGuid(), "Produto Teste", preco, quantidade);
    }

    // --- TESTES DE CRIAÇÃO ---

    [Fact(DisplayName = "Deve criar ItemPedido com sucesso quando dados válidos")]
    public void Criar_DeveRetornarItemPedido_QuandoDadosValidos()
    {
        var produtoId = Guid.NewGuid();
        var nomeProduto = "Teclado Mecânico";
        var precoUnitario = 250m;
        var quantidade = 2;

        var item = new ItemPedido(produtoId, nomeProduto, precoUnitario, quantidade);

        item.ProdutoId.Should().Be(produtoId);
        item.NomeProduto.Should().Be(nomeProduto);
        item.PrecoUnitario.Should().Be(precoUnitario);
        item.Quantidade.Should().Be(quantidade);
        item.DescontoAplicado.Should().Be(0);
        item.ValorTotal.Should().Be(500m);
    }

    [Theory(DisplayName = "Deve lançar DomainException quando parâmetros inválidos")]
    [InlineData("", "Produto A", 10, 1, "ProdutoId inválido.")]
    [InlineData("guid", "", 10, 1, "O nome do produto é obrigatório.")]
    [InlineData("guid", "Produto B", 0, 1, "O preço unitário deve ser maior que zero.")]
    [InlineData("guid", "Produto C", 10, 0, "A quantidade deve ser maior que zero.")]
    public void Criar_DeveLancarExcecao_QuandoParametrosInvalidos(
    string tipo, string nomeProduto, decimal preco, int qtd, string mensagem)
    {
        // Arrange
        var produtoId = tipo == "guid" ? Guid.NewGuid() : Guid.Empty;

        // Act
        Action act = () => new ItemPedido(produtoId, nomeProduto, preco, qtd);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(mensagem);
    }

    // --- TESTES DE DESCONTO ---
    [Fact(DisplayName = "Deve aplicar desconto com sucesso quando valor válido")]
    public void AplicarDesconto_DeveAplicarComSucesso_QuandoValorValido()
    {
        // Arrange
        var item = CriarItemValido(preco: 200m, quantidade: 2);

        // Act
        item.AplicarDesconto(50m);

        // Assert
        item.DescontoAplicado.Should().Be(50m);
        item.ValorTotal.Should().Be(350m); // (200 * 2) - 50
        item.DataAtualizacao.Should().NotBeNull();
    }

    [Theory(DisplayName = "Deve lançar exceção ao aplicar desconto inválido")]
    [InlineData(-10, "Desconto não pode ser negativo.")]
    [InlineData(1000, "Desconto não pode exceder o valor total do item.")]
    public void AplicarDesconto_DeveLancarExcecao_QuandoValorInvalido(decimal desconto, string mensagem)
    {
        // Arrange
        var item = CriarItemValido(preco: 100m, quantidade: 2);

        // Act
        Action act = () => item.AplicarDesconto(desconto);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(mensagem);
    }

    // --- TESTES DE ADIÇÃO DE UNIDADES ---
    [Fact(DisplayName = "Deve adicionar unidades com sucesso quando valor válido")]
    public void AdicionarUnidades_DeveAdicionarComSucesso_QuandoValorValido()
    {
        // Arrange
        var item = CriarItemValido(preco: 50m, quantidade: 2);

        // Act
        item.AdicionarUnidades(3);

        // Assert
        item.Quantidade.Should().Be(5);
        item.ValorTotal.Should().Be(250m);
        item.DataAtualizacao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve lançar exceção ao adicionar unidades inválidas")]
    public void AdicionarUnidades_DeveLancarExcecao_QuandoValorInvalido()
    {
        // Arrange
        var item = CriarItemValido();

        // Act
        Action act = () => item.AdicionarUnidades(0);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Deve-se adicionar pelo menos uma unidade.");
    }
    // --- TESTES DE REMOÇÃO DE UNIDADES ---
    [Fact(DisplayName = "Deve remover unidades com sucesso quando valor válido")]
    public void RemoverUnidades_DeveRemoverComSucesso_QuandoValorValido()
    {
        // Arrange
        var item = CriarItemValido(preco: 100m, quantidade: 5);

        // Act
        item.RemoverUnidades(2);

        // Assert
        item.Quantidade.Should().Be(3);
        item.ValorTotal.Should().Be(300m);
        item.DataAtualizacao.Should().NotBeNull();
    }

    [Theory(DisplayName = "Deve lançar exceção ao remover unidades inválidas")]
    [InlineData(0, "Deve-se remover pelo menos uma unidade.")]
    [InlineData(10, "Não é possível remover mais unidades do que existem no item.")]
    public void RemoverUnidades_DeveLancarExcecao_QuandoValorInvalido(int unidades, string mensagem)
    {
        // Arrange
        var item = CriarItemValido(preco: 100m, quantidade: 3);

        // Act
        Action act = () => item.RemoverUnidades(unidades);

        // Assert
        act.Should().Throw<DomainException>().WithMessage(mensagem);
    }

    [Fact(DisplayName = "Deve lançar exceção ao remover unidades e zerar quantidade")]
    public void RemoverUnidades_DeveLancarExcecao_QuandoQuantidadeZerar()
    {
        // Arrange
        var item = CriarItemValido(preco: 100m, quantidade: 2);

        // Act
        Action act = () => item.RemoverUnidades(2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Um item de pedido não pode ter quantidade zero. Use o método da classe Pedido para removê-lo.");
    }

    // --- TESTES DE ATUALIZAÇÃO DE PREÇO ---
    [Fact(DisplayName = "Deve atualizar preço unitário com sucesso quando valor válido")]
    public void AtualizarPrecoUnitario_DeveAtualizarComSucesso_QuandoValorValido()
    {
        // Arrange
        var item = CriarItemValido(preco: 100m, quantidade: 3);

        // Act
        item.AtualizarPrecoUnitario(150m);

        // Assert
        item.PrecoUnitario.Should().Be(150m);
        item.ValorTotal.Should().Be(450m);
        item.DataAtualizacao.Should().NotBeNull();
    }
    [Fact(DisplayName = "Deve lançar exceção ao atualizar preço unitário inválido")]
    public void AtualizarPrecoUnitario_DeveLancarExcecao_QuandoValorInvalido()
    {
        // Arrange
        var item = CriarItemValido();

        // Act
        Action act = () => item.AtualizarPrecoUnitario(0);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O preço unitário deve ser maior que zero.");
    }
    // --- TESTE DE IGUALDADE ENTRE ENTIDADES ---
    [Fact(DisplayName = "Dois itens com mesmo Id devem ser considerados iguais")]
    public void Equals_DeveRetornarTrue_QuandoMesmoId()
    {
        // Arrange
        var item1 = CriarItemValido();
        var item2 = CriarItemValido();

        // Forçar o mesmo Id por reflexão
        typeof(Entity).GetProperty("Id")!.SetValue(item2, item1.Id);

        // Act & Assert
        (item1 == item2).Should().BeTrue();
        item1.Equals(item2).Should().BeTrue();
    }
}
