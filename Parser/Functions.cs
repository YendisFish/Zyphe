namespace Zyphe.Parser;

public record FunctionSignature(
    Tuple<VariableIdentifier, TypeInfo> returnType, 
    string name, 
    bool isPrivate = false, 
    List<Generic>? generics = null
);