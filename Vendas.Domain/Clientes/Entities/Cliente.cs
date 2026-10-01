using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Events;
using Vendas.Domain.Clientes.ValueObjects;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Clientes.Entities;

public sealed class Cliente : AggregateRoot
{
    public NomeCompleto Nome { get; private set; }
    public Cpf Cpf { get; private set; }
    public Email Email { get; private set; }
    public Telefone Telefone { get; private set; }
    public StatusCliente Status { get; private set; }
    public Sexo Sexo { get; private set; }
    public EstadoCivil EstadoCivil { get; private set; }

    public Guid EnderecoPrincipalId { get; private set; }

    private readonly List<Endereco> _enderecos = new();
    public IReadOnlyCollection<Endereco> Enderecos => _enderecos.AsReadOnly();

    public Cliente(
        NomeCompleto nome,
        Cpf cpf,
        Email email,
        Telefone telefone,
        Endereco enderecoPrincipal,
        Sexo sexo = Sexo.NaoInformado,
        EstadoCivil estadoCivil = EstadoCivil.NaoInformado)
    {
        Validar(nome, cpf, email, telefone, enderecoPrincipal);

        Nome = nome;
        Cpf = cpf;
        Email = email;
        Telefone = telefone;
        Status = StatusCliente.Ativo;

        Sexo = sexo;
        EstadoCivil = estadoCivil;

        _enderecos.Add(enderecoPrincipal);
        EnderecoPrincipalId = enderecoPrincipal.Id;

        AddDomainEvent(new ClienteCadastradoEvent(
            ClienteId: Id,
            Nome: Nome.NomeCompletoFormatado,
            Cpf: Cpf.Numero,
            Email: Email.Endereco));
    }

    public void AdicionarEndereco(Endereco endereco)
    {
        Guard.AgainstNull(endereco, nameof(endereco), "Endereço inválido.");
        _enderecos.Add(endereco);
        SetDataAtualizacao();
    }
    public void RemoverEndereco(Guid enderecoId)
    {
        var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId);
        Guard.AgainstNull(endereco, nameof(endereco), "Endereço não encontrado.");

        Guard.Against<DomainException>(
            _enderecos.Count == 1,
            "O cliente deve possuir ao menos um endereço.");

        _enderecos.Remove(endereco!);

        // Se removeu o principal, escolhe outro automaticamente
        if (enderecoId == EnderecoPrincipalId)
        {
            EnderecoPrincipalId = _enderecos.First().Id;

            AddDomainEvent(new EnderecoPrincipalAlteradoEvent(
                ClienteId: Id,
                NovoEnderecoId: EnderecoPrincipalId));
        }

        SetDataAtualizacao();
    }
    public void AlterarEndereco(
    Guid enderecoId,
    string cep,
    string logradouro,
    string numero,
    string bairro,
    string cidade,
    string estado,
    string pais,
    string complemento = "")
    {
        var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId);
        Guard.AgainstNull(endereco, nameof(endereco), "Endereço não encontrado.");

        endereco!.Atualizar(cep, logradouro, numero, bairro, cidade, estado, pais, complemento);

        SetDataAtualizacao();
    }
    public void DefinirEnderecoPrincipal(Guid enderecoId)
    {
        var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId);
        Guard.AgainstNull(endereco, nameof(endereco), "Endereço não encontrado.");

        EnderecoPrincipalId = endereco!.Id;

        AddDomainEvent(new EnderecoPrincipalAlteradoEvent(
            ClienteId: Id,
            NovoEnderecoId: EnderecoPrincipalId));

        SetDataAtualizacao();
    }
    public Endereco ObterEnderecoPrincipal()
    {
        return _enderecos.First(e => e.Id == EnderecoPrincipalId);
    }


    public void AtualizarPerfil(
           NomeCompleto nome,
           Email email,
           Telefone telefone,
           Sexo sexo,
           EstadoCivil estadoCivil)
    {
        Guard.Against<DomainException>(
            Status == StatusCliente.Bloqueado,
            "Clientes bloqueados não podem atualizar o perfil.");

        Guard.AgainstNull(nome, nameof(nome));
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNull(telefone, nameof(telefone));

        Nome = nome;
        Email = email;
        Telefone = telefone;

        Sexo = sexo;
        EstadoCivil = estadoCivil;

        SetDataAtualizacao();
    }
    public void Bloquear()
    {
        if (Status == StatusCliente.Bloqueado)
            return;

        Status = StatusCliente.Bloqueado;

        AddDomainEvent(new ClienteBloqueadoEvent(
            ClienteId: Id,
            Cpf: Cpf.Numero));

        SetDataAtualizacao();
    }
    public void Ativar()
    {
        Status = StatusCliente.Ativo;
        SetDataAtualizacao();
    }
    private static void Validar(
    NomeCompleto nome,
    Cpf cpf,
    Email email,
    Telefone telefone,
    Endereco endereco)
    {
        Guard.AgainstNull(nome, nameof(nome));
        Guard.AgainstNull(cpf, nameof(cpf));
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNull(telefone, nameof(telefone));
        Guard.AgainstNull(endereco, nameof(endereco), "O cliente deve ter um endereço principal.");
    }
}
