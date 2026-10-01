using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalogo.Events;

public sealed record ImagemAdicionadaEvent(Guid ProdutoId, string Url, int Ordem)
    : DomainEventBase;