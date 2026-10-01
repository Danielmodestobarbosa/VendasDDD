using Vendas.Domain.Catalogo.Events;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalogo.Entities;

public sealed class Categoria : AggregateRoot
{
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public bool Ativa { get; private set; }

    public Categoria(string nome, string? descricao = null)
    {
        Guard.AgainstNullOrWhiteSpace(nome, nameof(nome), "Nome é obrigatório.");
        Guard.Against<DomainException>(nome.Length < 3, "Nome deve ter ao menos 3 caracteres.");

        Nome = nome.Trim();
        Descricao = descricao;
        Ativa = true;
    }

    public void AlterarNome(string novoNome)
    {
        Guard.AgainstNullOrWhiteSpace(novoNome, nameof(novoNome), "Nome é obrigatório.");
        Guard.Against<DomainException>(novoNome.Length < 3, "Nome deve ter ao menos 3 caracteres.");

        Nome = novoNome.Trim();
        SetDataAtualizacao();
    }
    public void AlterarDescricao(string? novaDescricao)
    {
        Descricao = novaDescricao;
        SetDataAtualizacao();
    }
    public void Ativar()
    {
        Guard.Against<DomainException>(Ativa, "Categoria já está ativa.");

        Ativa = true;
        SetDataAtualizacao();
        AddDomainEvent(new CategoriaAtivadaEvent(Id));
    }
    public void Inativar()
    {
        Guard.Against<DomainException>(!Ativa, "Categoria já está inativa.");

        Ativa = false;
        SetDataAtualizacao();
        AddDomainEvent(new CategoriaInativadaEvent(Id));
    }
}

