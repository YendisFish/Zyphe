namespace Zyphe.Parser;

public record FunctionSignature(
    Tuple<VariableIdentifier, TypeInfo> returnType,
    string name,
    bool isPrivate,
    List<VariableInfo>? arguments = null,
    List<Generic>? generics = null,
    bool isExtern = false)
{
    public static FunctionSignature GetterDefault(Declaration.VariableDeclaration decl) =>
        new FunctionSignature(new Tuple<VariableIdentifier, TypeInfo>(decl.left.vType, decl.left.type), 
            decl.left.name + "_getter", decl.left.isPrivate, new());
    
    public static FunctionSignature SetterDefault(Declaration.VariableDeclaration decl) =>
        new FunctionSignature(new Tuple<VariableIdentifier, TypeInfo>(decl.left.vType, decl.left.type), 
            decl.left.name + "_setter", decl.left.isPrivate, new());
}