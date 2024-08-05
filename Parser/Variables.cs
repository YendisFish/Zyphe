namespace Zyphe.Parser;

public record VariableInfo(
    VariableIdentifier vType,
    string name,
    TypeInfo type,
    bool isProp = false,
    bool isPrivate = false
)
{
    public Declaration.FunctionDeclaration? getter { get; set; } = null;
    public Declaration.FunctionDeclaration? setter { get; set; } = null;
}

public record TypeInfo(string name, List<Generic>? generics = null);

public enum VariableIdentifier
{
    LET,
    CONST,
    REF,
    REFCONST,
    USING
}