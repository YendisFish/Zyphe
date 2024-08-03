namespace Zyphe.Parser;

public record Scope(Guid scopeId, List<Scope> children)
{
    public Scope? parent { get; set; }
    public AstNode? returnNode { get; set; } = null;
    public ParserState? returnState { get; set; } = null;

    public Scope() : this(Guid.NewGuid(), new List<Scope>())
    {
        parent = null;
    }
}