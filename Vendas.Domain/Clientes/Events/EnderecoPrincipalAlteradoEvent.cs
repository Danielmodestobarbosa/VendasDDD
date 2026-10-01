using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Clientes.Events;

public sealed record EnderecoPrincipalAlteradoEvent(
        Guid ClienteId,
        Guid NovoEnderecoId) : DomainEventBase;