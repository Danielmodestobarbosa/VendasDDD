using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalogo.Events;

public sealed record ProdutoInativadoEvent(Guid ProdutoId) : DomainEventBase;