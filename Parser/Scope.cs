namespace Zyphe.Parser;

public class Scope
{
    public Guid scopeId { get; set; }
    public Scope? parent { get; set; }
    public AstNode? returnNode { get; set; } = null;
    public ParserState? returnState { get; set; } = null;

    public Scope()
    {
        scopeId = Guid.NewGuid();
        parent = null;
    }
}