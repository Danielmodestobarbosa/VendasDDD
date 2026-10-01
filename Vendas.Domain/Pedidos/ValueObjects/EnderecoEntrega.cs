using System.Text.RegularExpressions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Pedidos.ValueObjects;

public class EnderecoEntrega : ValueObject
{
    public string Cep { get; private set; }
    public string Logradouro { get; private set; }
    public string Complemento { get; private set; }
    public string Bairro { get; private set; }
    public string Estado { get; private set; }
    public string Cidade { get; private set; }
    public string Pais { get; private set; }

    private EnderecoEntrega(string cep, string logradouro, string complemento,
                           string bairro, string estado, string cidade,
                           string pais)
    {
        Guard.AgainstNullOrWhiteSpace(cep, nameof(Cep));
        Guard.AgainstNullOrWhiteSpace(logradouro, nameof(Logradouro));
        Guard.AgainstNullOrWhiteSpace(bairro, nameof(Bairro));
        Guard.AgainstNullOrWhiteSpace(estado, nameof(Estado));
        Guard.AgainstNullOrWhiteSpace(cidade, nameof(Cidade));
        Guard.AgainstNullOrWhiteSpace(pais, nameof(Pais));

        // Validação de formato para CEP
        // (exemplo para Brasil; extensível para outros países)
        if (!Regex.IsMatch(cep ?? "", @"^\d{5}-\d{3}$"))
            throw new DomainException("CEP inválido. Deve ser no formato 00000-000.");

        Cep = cep!;
        Logradouro = logradouro;
        Complemento = complemento ?? string.Empty;  // Trata opcional
        Bairro = bairro;
        Estado = estado;
        Cidade = cidade;
        Pais = pais;
    }
    public static EnderecoEntrega Criar(string cep, string logradouro, string complemento, string bairro,
                                       string estado, string cidade, string pais)
    {
        return new EnderecoEntrega(cep, logradouro, complemento, bairro, estado, cidade, pais);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Cep;
        yield return Logradouro;
        yield return Complemento ?? string.Empty;
        yield return Bairro;
        yield return Estado;
        yield return Cidade;
        yield return Pais;
    }

    public string FormatarEndereco()
    {
        return $"{Logradouro}, {Complemento} - {Bairro}, {Cidade} - {Estado}, {Pais} - CEP: {Cep}";
    }
}
