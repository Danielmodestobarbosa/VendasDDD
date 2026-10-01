using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Pedidos.Events;

public sealed record PedidoEntregueEvent(
    Guid PedidoId,
    Guid ClienteId) : DomainEventBase;
