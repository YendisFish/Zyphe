namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeFixed()
    {
        index = index + 1;

        string name = (string)tokens[index].value;

        index = index + 2;

        Expression? len = null;
        TypeInfo tinf = ConsumeFixedType(ref len);

        Declaration.StackBufferDeclaration declaration = new(new VariableInfo(VariableIdentifier.LET, name), len);
        
        currentNode.children.Add(declaration);
        
        declaration.Scope = currentNode.Scope;
        declaration.Scope.parent = currentNode.Scope;
        declaration.Scope.returnNode = currentNode;
        declaration.parent = currentNode;
    }
    
    public TypeInfo ConsumeFixedType(ref Expression? expr)
    {
        string typeName = (string)tokens[index].value;
        List<GenericUsage> usages = null;
        bool isArray = false;
        
        //index = index + 1;
        
        if (tokens[index + 1].type == Token.TokenType.LALLIGATOR)
        {
            index = index + 1;
            usages = this.ConsumeGenericUsages() ?? throw new NullReferenceException();
        }

        index = index + 2;
        this.ConsumeExpression2(ref expr);
        
        return new TypeInfo(typeName, usages, isArray);
    }
}