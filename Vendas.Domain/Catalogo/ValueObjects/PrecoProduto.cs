using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalogo.ValueObjects;

public sealed class PrecoProduto : ValueObject
{
    public decimal Valor { get; }

    public PrecoProduto(decimal valor)
    {
        Guard.Against<DomainException>(valor <= 0,
            "O preço do produto deve ser maior que zero.");

        Valor = valor;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}