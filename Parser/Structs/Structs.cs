namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeStruct()
    {
        index = index + 1;
        
        string name = (string)tokens[index].value;
        currentTypeName = name;
        
        List<Generic> generics = null;
        
        if (tokens[index + 1].type == Token.TokenType.LALLIGATOR)
        {
            index = index + 2;
            generics = this.ConsumeGenerics() ?? throw new NullReferenceException();
            index = index + 1;
        }
                
        StructInfo sInfo = new StructInfo(name, generics, null);
        Declaration.StructDeclaration structDecl = new Declaration.StructDeclaration(sInfo);

        currentNode.children.Add(structDecl);
        currentNode = structDecl;

        state = ParserState.STRUCT;
    }
}