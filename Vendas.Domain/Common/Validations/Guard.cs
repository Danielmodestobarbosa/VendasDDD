using Vendas.Domain.Common.Exceptions;
namespace Vendas.Domain.Common.Validations;

internal static class Guard
{
    public static void AgainstEmptyGuid(Guid id, string paramName, string? message = null)
    {
        if (id == Guid.Empty)
            throw new DomainException(message ?? $"{paramName} não pode ser Guid.Empty.");
    }

    public static void AgainstNull<T>(T value, string paramName)
    {
        if (value == null)
            throw new DomainException($"{paramName} não pode ser nulo.");
    }
    public static void AgainstNull<T>(T value, string paramName, string message)
    {
        if (value == null)
            throw new DomainException(message);
    }

    public static void AgainstNullOrWhiteSpace(string value, string paramName, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(message ?? $"{paramName} não pode ser nulo ou vazio.");
    }


    public static void Against<TException>(bool condition, string message) where TException : Exception
    {
        if (condition) throw (TException)Activator.CreateInstance(typeof(TException), message)!;
    }
}
