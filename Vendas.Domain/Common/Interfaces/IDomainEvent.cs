namespace Vendas.Domain.Common.Interfaces;

public interface IDomainEvent
{
    DateTime DateOccurred { get; }
}
