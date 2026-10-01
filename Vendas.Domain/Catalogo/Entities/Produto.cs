using Vendas.Domain.Catalogo.Enums;
using Vendas.Domain.Catalogo.Events;
using Vendas.Domain.Catalogo.ValueObjects;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalogo.Entities;

public sealed class Produto : AggregateRoot
{
    public NomeProduto Nome { get; private set; }
    public CodigoProduto Codigo { get; private set; }
    public PrecoProduto Preco { get; private set; }
    public string? Descricao { get; private set; }
    public Guid CategoriaId { get; private set; }
    public StatusProduto Status { get; private set; }
    public int Estoque { get; private set; }

    private readonly List<ImagemProduto> _imagens = new();
    public IReadOnlyCollection<ImagemProduto> Imagens => _imagens.AsReadOnly();

    public Produto(
       NomeProduto nome,
       CodigoProduto codigo,
       PrecoProduto preco,
       Guid categoriaId,
       int estoqueInicial = 0,
       string? descricao = null)
    {
        Guard.AgainstNull(nome, nameof(nome));
        Guard.AgainstNull(codigo, nameof(codigo));
        Guard.AgainstNull(preco, nameof(preco));
        Guard.AgainstEmptyGuid(categoriaId, nameof(categoriaId));
        Guard.Against<DomainException>(estoqueInicial < 0,
            "Estoque inicial não pode ser negativo.");

        Nome = nome;
        Codigo = codigo;
        Preco = preco;
        CategoriaId = categoriaId;
        Descricao = descricao?.Trim();
        Estoque = estoqueInicial;

        Status = StatusProduto.Ativo;
    }
    public void AlterarNome(NomeProduto novoNome)
    {
        Guard.AgainstNull(novoNome, nameof(novoNome));
        Nome = novoNome;
        SetDataAtualizacao();
    }
    public void AlterarPreco(PrecoProduto novoPreco)
    {
        Guard.AgainstNull(novoPreco, nameof(novoPreco));

        var antigo = Preco.Valor;
        var novo = novoPreco.Valor;

        Preco = novoPreco;
        SetDataAtualizacao();

        AddDomainEvent(new PrecoProdutoAlteradoEvent(Id, antigo, novo));
    }
    public void AlterarCategoria(Guid novaCategoriaId)
    {
        Guard.AgainstEmptyGuid(novaCategoriaId, nameof(novaCategoriaId));
        CategoriaId = novaCategoriaId;
        SetDataAtualizacao();
    }
    public void AlterarDescricao(string? novaDescricao)
    {
        Descricao = novaDescricao?.Trim();
        SetDataAtualizacao();
    }

    public void AjustarEstoque(int quantidade, string motivo)
    {
        Guard.AgainstNullOrWhiteSpace(motivo, nameof(motivo));
        Guard.Against<DomainException>(Estoque + quantidade < 0,
            "O ajuste de estoque resultaria em valor negativo.");

        Estoque += quantidade;
        SetDataAtualizacao();

        AddDomainEvent(new EstoqueAjustadoEvent(Id, quantidade, motivo));
    }

    public void Ativar()
    {
        Guard.Against<DomainException>(Status == StatusProduto.Ativo,
            "O produto já está ativo.");

        Status = StatusProduto.Ativo;
        SetDataAtualizacao();

        AddDomainEvent(new ProdutoAtivadoEvent(Id));
    }

    public void Inativar()
    {
        Guard.Against<DomainException>(Status == StatusProduto.Inativo,
            "O produto já está inativado.");

        Status = StatusProduto.Inativo;
        SetDataAtualizacao();

        AddDomainEvent(new ProdutoInativadoEvent(Id));
    }
    public void AdicionarImagem(ImagemProduto imagem)
    {
        Guard.AgainstNull(imagem, nameof(imagem));

        Guard.Against<DomainException>(
            _imagens.Any(i => i.Ordem == imagem.Ordem),
            "Já existe uma imagem com esta ordem.");

        _imagens.Add(imagem);

        SetDataAtualizacao();
        AddDomainEvent(new ImagemAdicionadaEvent(Id, imagem.Url, imagem.Ordem));
    }
}
