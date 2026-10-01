using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalogo.Events;

public sealed record ProdutoAtivadoEvent(Guid ProdutoId) : DomainEventBase;
