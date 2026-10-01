using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Pedidos.Events;

public record PagamentoRejeitadoEvent(Guid PagamentoId,
                                      Guid PedidoId,
                                      decimal Valor,
                                      DateTime DataPagamento,
                                      string? CodigoTransacao) : DomainEventBase;
