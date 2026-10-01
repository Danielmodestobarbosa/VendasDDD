using Vendas.Domain.Common.Base;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Pedidos.Events;

public sealed record PedidoCanceladoEvent(
    Guid PedidoId,
    Guid ClienteId,
    StatusPedido StatusAnterior,
    MotivoCancelamento Motivo,
    Guid? PagamentoId) : DomainEventBase;
