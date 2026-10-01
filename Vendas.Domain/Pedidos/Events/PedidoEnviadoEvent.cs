using Vendas.Domain.Common.Base;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Pedidos.Events;

public sealed record PedidoEnviadoEvent(
    Guid PedidoId,
    Guid ClienteId,
    EnderecoEntrega EnderecoEntrega) : DomainEventBase;