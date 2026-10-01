using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalogo.ValueObjects;

public sealed class ImagemProduto : ValueObject
{
    public string Url { get; }
    public int Ordem { get; }

    public ImagemProduto(string url, int ordem)
    {
        Guard.AgainstNullOrWhiteSpace(url, nameof(url),
            "A URL da imagem é obrigatória.");

        Guard.Against<DomainException>(ordem < 1,
            "A ordem da imagem deve ser >= 1.");

        Url = url;
        Ordem = ordem;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Url;
        yield return Ordem;
    }
}