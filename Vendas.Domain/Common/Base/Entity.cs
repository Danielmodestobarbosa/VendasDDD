using Vendas.Domain.Common.Interfaces;

namespace Vendas.Domain.Common.Base;

public abstract class Entity
{
    // Usar protected set para que apenas a própria classe ou 
    // classes derivadas possam alterar o Id, reforçando a imutabilidade após a criação.
    public Guid Id { get; protected set; }
    public DateTime DataCriacao { get; protected set; }
    public DateTime? DataAtualizacao { get; protected set; }

    // Construtor protegido: Entidades devem ser criadas via um Construtor ou Factory, 
    // e não instanciadas de forma "simples".
    protected Entity()
    {
        // Atribui um novo Guid no momento da criação
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
    }
    protected void SetDataAtualizacao()
    {
        DataAtualizacao = DateTime.UtcNow;
    }
    // Você pode querer um construtor que receba o Id (útil para ORMs)
    protected Entity(Guid id)
    {
        Id = id;
    }
    // Sobrescrever Equals e GetHashCode é crucial para comparar Entidades
    // baseado APENAS na sua identidade (Id).
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;

        // Se o Id ainda não foi persistido (é Guid.Empty), compara por referência.
        // No nosso caso, como inicializamos no construtor, essa checagem pode ser simplificada
        // para apenas comparar o Id.
        return Id.Equals(other.Id);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public static bool operator ==(Entity left, Entity right)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    public void ClearDomainEvents() => _domainEvents.Clear();
}
