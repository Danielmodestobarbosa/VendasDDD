using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Clientes.ValueObjects;

public sealed class Cpf : ValueObject
{
    public string Numero { get; }

    public Cpf(string numero)
    {
        Guard.AgainstNullOrWhiteSpace(numero, nameof(numero), "O CPF é obrigatório.");

        var digits = new string(numero.Where(char.IsDigit).ToArray());

        Guard.Against<DomainException>(digits.Length != 11, "CPF deve conter 11 dígitos.");
        Guard.Against<DomainException>(!CpfValido(digits), "CPF inválido.");

        Numero = digits;
    }

    public override string ToString()
        => Convert.ToUInt64(Numero).ToString(@"000\.000\.000\-00");

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
    }

    private static bool CpfValido(string cpf)
    {
        // Rejeitar CPFs repetidos
        if (new string(cpf[0], cpf.Length) == cpf)
            return false;

        int Soma(int length, int weight)
        {
            int sum = 0;
            for (int i = 0; i < length; i++)
                sum += (cpf[i] - '0') * (weight - i);
            return sum;
        }

        int dv1 = Soma(9, 10) % 11;
        dv1 = dv1 < 2 ? 0 : 11 - dv1;

        int dv2 = Soma(10, 11) % 11;
        dv2 = dv2 < 2 ? 0 : 11 - dv2;

        return cpf[9] - '0' == dv1 && cpf[10] - '0' == dv2;
    }
}
