using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Catalogo.ValueObjects;

public sealed class NomeProduto : ValueObject
{
    public string Valor { get; }

    public NomeProduto(string valor)
    {
        Guard.AgainstNullOrWhiteSpace(valor, nameof(valor),
            "O nome do produto é obrigatório.");

        Guard.Against<DomainException>(valor.Length < 3,
            "O nome do produto deve ter ao menos 3 caracteres.");

        Guard.Against<DomainException>(valor.Length > 150,
            "O nome do produto deve ter no máximo 150 caracteres.");

        Valor = valor.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}