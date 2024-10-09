namespace Zyphe.Parser;

public record VariableInfo(
    VariableIdentifier vType,
    string name,
    TypeInfo type,
    bool isProp = false,
    bool isPrivate = false,
    bool isStatic = false,
    bool isForIterator = false
)
{
    public Declaration.FunctionDeclaration? getter { get; set; } = null;
    public Declaration.FunctionDeclaration? setter { get; set; } = null;
}

public record TypeInfo(string name, List<GenericUsage>? generics = null, bool isArray = false);

public enum VariableIdentifier
{
    LET,
    CONST,
    REF,
    REFCONST,
    USING
}