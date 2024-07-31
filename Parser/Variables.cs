namespace Zyphe.Parser;

public record VariableInfo(
    VariableIdentifier vType, 
    string name, 
    string type,
    Declaration.FunctionDeclaration? getter, 
    Declaration.FunctionDeclaration? setter,
    bool isProp = false
);

public enum VariableIdentifier
{
    LET,
    CONST,
    REF,
    REFCONST,
    USING
}