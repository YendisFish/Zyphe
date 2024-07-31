namespace Zyphe.Parser;

public record Scope(Guid scopeId, Scope? parent, List<Scope> children)
{
    public Scope() : this(Guid.NewGuid(), null, new List<Scope>()) { }
}