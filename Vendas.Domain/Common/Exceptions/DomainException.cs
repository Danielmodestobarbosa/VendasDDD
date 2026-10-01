namespace Vendas.Domain.Common.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    // Método estático para validações de "pré-condição"
    public static void When(bool hasError, string errorMessage)
    {
        if (hasError)
        {
            throw new DomainException(errorMessage);
        }
    }
}
