using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Clientes.ValueObjects;

public sealed class Telefone : ValueObject
{
    public string Numero { get; }

    public Telefone(string numero)
    {
        Guard.AgainstNullOrWhiteSpace(numero, nameof(numero), "O telefone é obrigatório.");

        var digits = new string(numero.Where(char.IsDigit).ToArray());

        Guard.Against<DomainException>(
            digits.Length is < 10 or > 11,
            "Telefone deve conter 10 (fixo) ou 11 dígitos (celular).");

        Numero = digits;
    }

    public override string ToString()
    {
        // Formatação automática: (99) 99999-9999 ou (99) 9999-9999
        if (Numero.Length == 11)
            return Convert.ToUInt64(Numero).ToString(@"\(00\) 00000\-0000");

        return Convert.ToUInt64(Numero).ToString(@"\(00\) 0000\-0000");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
    }
}
