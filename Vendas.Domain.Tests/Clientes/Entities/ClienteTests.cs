using FluentAssertions;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Events;
using Vendas.Domain.Clientes.ValueObjects;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Clientes.Entities;

public class ClienteTests
{
    private static NomeCompleto CriarNomeCompleto(string nome = "João Silva")
    => new(nome);

    private static Cpf CriarCpf(string cpf = "12345678909")
        => new(cpf);

    private static Email CriarEmail(string email = "joao@example.com")
        => new(email);

    private static Telefone CriarTelefone(string telefone = "11999999999")
        => new(telefone);

    private static Endereco CriarEndereco(
        string cep = "01310100",
        string logradouro = "Avenida Paulista",
        string numero = "1000",
        string bairro = "Bela Vista",
        string cidade = "São Paulo",
        string estado = "SP",
        string pais = "Brasil",
        string complemento = "")
        => new(cep, logradouro, numero, bairro, cidade, estado, pais, complemento);

    private static Cliente CriarClienteValido()
        => new Cliente(
            CriarNomeCompleto(),
            CriarCpf(),
            CriarEmail(),
            CriarTelefone(),
            CriarEndereco(),
            Sexo.Masculino,
            EstadoCivil.Solteiro);


    [Fact]
    public void Construtor_ComDadosValidos_DeveCriarCliente()
    {
        var cliente = CriarClienteValido();

        cliente.Status.Should().Be(StatusCliente.Ativo);
        cliente.Sexo.Should().Be(Sexo.Masculino);
        cliente.EstadoCivil.Should().Be(EstadoCivil.Solteiro);

        cliente.Enderecos.Should().ContainSingle();
        cliente.EnderecoPrincipalId.Should().Be(cliente.Enderecos.First().Id);
    }
    [Fact]
    public void Construtor_DeveGerarEventoClienteCadastrado()
    {
        var cliente = CriarClienteValido();

        cliente.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ClienteCadastradoEvent>();
    }


    [Theory]
    [InlineData("Nome")]
    [InlineData("Cpf")]
    [InlineData("Email")]
    [InlineData("Telefone")]
    [InlineData("Endereco")]
    public void Construtor_ComParametroObrigatorioNulo_DeveLancarDomainException(string campo)
    {
        NomeCompleto? nome = campo == "Nome" ? null : CriarNomeCompleto();
        Cpf? cpf = campo == "Cpf" ? null : CriarCpf();
        Email? email = campo == "Email" ? null : CriarEmail();
        Telefone? telefone = campo == "Telefone" ? null : CriarTelefone();
        Endereco? endereco = campo == "Endereco" ? null : CriarEndereco();

        Action act = () => new Cliente(nome!, cpf!, email!, telefone!, endereco!);

        act.Should().Throw<DomainException>();
    }
    [Fact]
    public void AdicionarEndereco_DeveAdicionar()
    {
        var cliente = CriarClienteValido();
        var novo = CriarEndereco("02134000", "Rua Augusta");

        cliente.AdicionarEndereco(novo);

        cliente.Enderecos.Should().HaveCount(2);
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AdicionarEndereco_ValidacaoDeNulo(bool usarNulo)
    {
        var cliente = CriarClienteValido();
        Endereco? endereco = usarNulo ? null : CriarEndereco();

        Action act = () => cliente.AdicionarEndereco(endereco!);

        if (usarNulo)
            act.Should().Throw<DomainException>();
        else
            act.Should().NotThrow();
    }
    [Fact]
    public void AdicionarEndereco_DeveAtualizarDataModificacao()
    {
        var cliente = CriarClienteValido();
        var dataAnterior = cliente.DataAtualizacao ?? DateTime.UtcNow;

        System.Threading.Thread.Sleep(5);
        cliente.AdicionarEndereco(CriarEndereco("02134000", "Rua Augusta"));
        cliente.DataAtualizacao.Should().BeAfter(dataAnterior);
    }
    [Fact]
    public void RemoverEndereco_ComSegundoEndereco_DeveRemover()
    {
        var cliente = CriarClienteValido();
        var segundo = CriarEndereco("02134000", "Rua Augusta");
        cliente.AdicionarEndereco(segundo);

        cliente.RemoverEndereco(segundo.Id);

        cliente.Enderecos.Should().HaveCount(1);
    }
    [Theory]
    [InlineData("NaoExiste")]
    [InlineData("Ultimo")]
    public void RemoverEndereco_DeveLancarExceptions(string caso)
    {
        var cliente = CriarClienteValido();
        Guid id = caso switch
        {
            "NaoExiste" => Guid.NewGuid(),
            "Ultimo" => cliente.EnderecoPrincipalId,
            _ => throw new ArgumentOutOfRangeException()
        };

        Action act = () => cliente.RemoverEndereco(id);

        act.Should().Throw<DomainException>();
    }
   

}

