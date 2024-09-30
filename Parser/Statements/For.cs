namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeForLoop(ParserState returnState)
    {
        state = ParserState.FOR;

        switch (tokens[index].type)
        {
            case Token.TokenType.RPAREN:
            {
                break;
            }
            case Token.TokenType.LBRACK:
            {
                index = index + 1;
                this.ReadObjForHeader(returnState);
                break;
            }
        }
        
        //state = returnState;
    }

    public void ReadIterForHeader()
    {
        //todo : implement iterator headers
    }

    public void ReadObjForHeader(ParserState returnState)
    {
        VariableIdentifier ident = (tokens[index].keyword == Token.KeywordType.LET) ? 
            VariableIdentifier.LET : VariableIdentifier.REF;

        TypeInfo? tinf = null;

        index = index + 1;
        string name = (string)tokens[index].value;
        index = index + 1;
        
        if (tokens[index].type == Token.TokenType.COLON)
        {
            index = index + 1;
            tinf = this.ConsumeVarType();
            //index = index + 1;
        }

        index = index + 1;
        Expression expr = null;
        this.ConsumeExpression2(ref expr);
        index = index + 1;

        VariableInfo? vInfo = new VariableInfo(ident, name, tinf, isForIterator: true);
        Declaration.VariableDeclaration decl = new Declaration.VariableDeclaration(vInfo, expr);

        Statement.ForStatement2 f = new Statement.ForStatement2(decl);
        
        f.Scope.returnState = returnState;
        f.Scope.returnNode = currentNode;
        f.parent = currentNode;
        f.Scope.parent = currentNode.Scope;
        
        currentNode.children.Add(f);
        currentNode = f;
    }
}