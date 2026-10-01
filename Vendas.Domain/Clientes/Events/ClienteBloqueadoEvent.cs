using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Clientes.Events;

public sealed record ClienteBloqueadoEvent(
        Guid ClienteId,
        string Cpf) : DomainEventBase;