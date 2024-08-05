namespace Zyphe.Parser;

public record FunctionSignature(
    Tuple<VariableIdentifier, TypeInfo> returnType,
    string name,
    bool isPrivate = false,
    List<Generic>? generics = null)
{
    public static FunctionSignature GetterDefault(Declaration.VariableDeclaration decl) =>
        new FunctionSignature(new Tuple<VariableIdentifier, TypeInfo>(decl.left.vType, decl.left.type), 
            decl.left.name + "_getter", decl.left.isPrivate);
    
    public static FunctionSignature SetterDefault(Declaration.VariableDeclaration decl) =>
        new FunctionSignature(new Tuple<VariableIdentifier, TypeInfo>(decl.left.vType, decl.left.type), 
            decl.left.name + "_setter", decl.left.isPrivate);
}