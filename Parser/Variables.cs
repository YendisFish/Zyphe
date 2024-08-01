namespace Zyphe.Parser;

public record VariableInfo(
    VariableIdentifier vType, 
    string name, 
    TypeInfo type,
    Declaration.FunctionDeclaration? getter, 
    Declaration.FunctionDeclaration? setter,
    bool isProp = false
);

public record TypeInfo(string name, List<Generic>? generics = null);

public enum VariableIdentifier
{
    LET,
    CONST,
    REF,
    REFCONST,
    USING
}