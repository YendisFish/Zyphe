namespace Zyphe.Parser;

public record FunctionSignature(
    Tuple<VariableIdentifier, string> returnType, 
    string name, 
    bool isPrivate = false, 
    List<Generic>? generics = null
);