namespace Zyphe.Parser;

public record StructInfo(
    string name, 
    List<Generic>? generics = null, 
    List<Constraint>? constraints = null, 
    bool isBound = false,
    VariableIdentifier? identifier = null);