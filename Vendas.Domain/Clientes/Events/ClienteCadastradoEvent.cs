using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Clientes.Events;

public sealed record ClienteCadastradoEvent(
        Guid ClienteId,
        string Nome,
        string Cpf,
        string Email) : DomainEventBase;