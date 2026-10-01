namespace Vendas.Domain.Common.Base;
public abstract class ValueObject
{
    // Método que deve ser implementado nas classes filhas para retornar 
    // todos os atributos que definem a igualdade estrutural.
    protected abstract IEnumerable<object> GetEqualityComponents();

    // Implementação de igualdade profunda (estrutural)
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    public override int GetHashCode()
    {
        // Combina os HashCodes de todos os componentes
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject a, ValueObject b)
    => a?.Equals(b) ?? b is null;

    public static bool operator != (ValueObject a, ValueObject b)
        => !(a == b);
}
