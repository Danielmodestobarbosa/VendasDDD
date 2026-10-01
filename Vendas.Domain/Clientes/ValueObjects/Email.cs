using System.Text.RegularExpressions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Clientes.ValueObjects;

public sealed class Email : ValueObject
{
    public string Endereco { get; }

    private static readonly Regex _regex = new(
        @"^[\w\.-]+@[\w\.-]+\.\w{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string endereco)
    {
        Guard.AgainstNullOrWhiteSpace(endereco, nameof(endereco), "O email é obrigatório.");
        Guard.Against<DomainException>(!_regex.IsMatch(endereco), "Email inválido.");

        Endereco = endereco.Trim().ToLowerInvariant();
    }

    public override string ToString() => Endereco;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Endereco;
    }
}
