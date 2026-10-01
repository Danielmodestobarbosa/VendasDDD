using FluentAssertions;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Tests.Pedidos.ValueObjects;

public class EnderecoEntregaTests
{
    // Testes de criação bem-sucedida
    //------------------------------------
    [Fact(DisplayName = "Deve criar EnderecoEntrega com sucesso quando todos os dados são válidos")]
    public void Criar_DeveRetornarEnderecoValido_QuandoDadosForemValidos()
    {
        // Arrange
        var cep = "12345-678";
        var logradouro = "Rua das Flores";
        var complemento = "Apto 101";
        var bairro = "Centro";
        var estado = "SP";
        var cidade = "São Paulo";
        var pais = "Brasil";

        // Act
        var endereco = EnderecoEntrega.Criar(cep, logradouro, complemento, bairro, estado, cidade, pais);

        // Assert
        endereco.Should().NotBeNull();
        endereco.Cep.Should().Be(cep);
        endereco.Logradouro.Should().Be(logradouro);
        endereco.Complemento.Should().Be(complemento);
        endereco.Bairro.Should().Be(bairro);
        endereco.Estado.Should().Be(estado);
        endereco.Cidade.Should().Be(cidade);
        endereco.Pais.Should().Be(pais);
        endereco.FormatarEndereco().Should().Contain("Rua das Flores");
    }

    //Testes de falha na criação - formatação de cep   
    //------------------------------------------------

    [Theory(DisplayName = "Deve lançar DomainException quando o CEP for inválido")]
    [InlineData("12345678")]   // sem hífen
    [InlineData("12-345678")]  // formato incorreto
    [InlineData("ABCDE-123")]  // caracteres inválidos
    public void Criar_DeveLancarDomainException_QuandoCepForInvalido(string cepInvalido)
    {
        // Arrange
        var logradouro = "Rua das Flores";
        var complemento = "Casa";
        var bairro = "Centro";
        var estado = "SP";
        var cidade = "São Paulo";
        var pais = "Brasil";

        // Act
        Action act = () => EnderecoEntrega.Criar(cepInvalido, logradouro, complemento, bairro, estado, cidade, pais);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CEP inválido*");
    }

    [Fact(DisplayName = "Dois EnderecosEntrega com mesmos dados devem ser iguais (Value Object)")]
    public void EnderecosDevemSerIguais_QuandoPossuemMesmosValores()
    {
        // Arrange
        var endereco1 = EnderecoEntrega.Criar("12345-678", "Rua X", "Casa", "Centro", "SP", "São Paulo", "Brasil");
        var endereco2 = EnderecoEntrega.Criar("12345-678", "Rua X", "Casa", "Centro", "SP", "São Paulo", "Brasil");

        // Assert
        endereco1.Should().Be(endereco2);
        (endereco1 == endereco2).Should().BeTrue();
    }

    [Fact(DisplayName = "EnderecosEntrega devem ser diferentes quando algum campo for diferente")]
    public void EnderecosDevemSerDiferentes_QuandoAlgumCampoForDiferente()
    {
        // Arrange
        var endereco1 = EnderecoEntrega.Criar("12345-678", "Rua X", "Casa", "Centro", "SP", "São Paulo", "Brasil");
        var endereco2 = EnderecoEntrega.Criar("12345-678", "Rua Y", "Casa", "Centro", "SP", "São Paulo", "Brasil");

        // Assert
        endereco1.Should().NotBe(endereco2);
    }

    [Fact(DisplayName = "EnderecoEntrega deve ser imutável após criação")]
    public void EnderecoDeveSerImutavel_AposCriacao()
    {
        // Arrange
        var endereco = EnderecoEntrega.Criar("12345-678", "Rua X", "Casa", "Centro", "SP", "São Paulo", "Brasil");

        // Act
        Action act = () =>
        {
            // Tentativa hipotética (não compila, apenas conceitual)
            //endereco.Cep = "99999-999"; 
        };

        // Assert
        endereco.GetType().GetProperties()
            .All(p => p.SetMethod == null || p.SetMethod.IsPrivate)
            .Should().BeTrue("as propriedades do VO devem ser imutáveis");
    }

    [Theory(DisplayName = "Deve lançar DomainException quando campos obrigatórios forem nulos ou vazios")]
    [InlineData(null, "Logradouro", "Bairro", "Estado", "Cidade", "Pais")] // CEP nulo
    [InlineData("12345-678", null, "Bairro", "Estado", "Cidade", "Pais")] // Logradouro nulo
    [InlineData("12345-678", "Logradouro", "Bairro", "Estado", "Cidade", null)] // Pais nulo
    public void Criar_DeveLancarDomainException_QuandoCamposObrigatoriosNulosOuVazios(
     string? cep, string? logradouro, string? bairro, string estado, string cidade, string? pais)
    {
        // Act
        Action act = () => EnderecoEntrega.Criar(cep!, logradouro!, "Complemento", bairro!, estado, cidade, pais!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*não pode ser nulo ou vazio*");
    }
}
