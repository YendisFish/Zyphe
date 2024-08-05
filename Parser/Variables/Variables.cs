namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeLetVariable(ParserState returnState)
    {
        state = ParserState.VARIABLE;
        
        string name = (string)tokens[index].value;
        TypeInfo? tInfo = null;
        Expression? expr = null;
        index = index + 1;

        if (tokens[index].type == Token.TokenType.COLON)
        {
            index = index + 1;
            tInfo = this.ConsumeVarType();
        }
        
        switch (tokens[index].type)
        {
            case Token.TokenType.EQUALS:
            {
                index = index + 1;
                expr = this.ConsumeExpression();
                index = index + 1;
                break;
            }

            case Token.TokenType.SEMICOLON:
            {
                index = index + 1;
                break;
            }
        }

        if (tInfo is null)
        {
            // todo : type identification engine, little bit of semantics fr
        }

        VariableInfo info = new VariableInfo(VariableIdentifier.LET, name, tInfo, false, readingPrivateScope);
        Declaration.VariableDeclaration declaration = new Declaration.VariableDeclaration(info, expr);

        declaration.Scope = currentNode.Scope;
        declaration.parent = currentNode;
        
        declaredVariables.Add(name);
        currentNode.children.Add(declaration);

        state = returnState;
        
        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
    }

    public void ConsumeRefVariable(ParserState returnState)
    {
        string name = (string)tokens[index].value;
        TypeInfo? tInfo = null;
        Expression? expr = null;
        index = index + 1;
        
        if (tokens[index].type == Token.TokenType.COLON)
        {
            index = index + 1;
            tInfo = this.ConsumeVarType();
        }
        
        switch (tokens[index].type)
        {
            case Token.TokenType.EQUALS:
            {
                index = index + 1;
                expr = this.ConsumeExpression();
                index = index + 1;
                break;
            }

            case Token.TokenType.SEMICOLON:
            {
                index = index + 1;
                break;
            }
        }

        if (tInfo is null)
        {
            // todo : type identification engine, little bit of semantics fr
        }

        VariableInfo info = new VariableInfo(VariableIdentifier.REF, name, tInfo, false, readingPrivateScope);
        Declaration.VariableDeclaration declaration = new Declaration.VariableDeclaration(info, expr);

        declaration.Scope = currentNode.Scope;
        declaration.parent = currentNode;
        
        declaredVariables.Add(name);
        currentNode.children.Add(declaration);

        state = returnState;
        
        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
    }
    
    public TypeInfo ConsumeVarType(Token.TokenType readToOverride = Token.TokenType.EQUALS)
    {
        string typeName = (string)tokens[index].value;
        
        if (tokens[index].type == Token.TokenType.LALLIGATOR)
        {
            // todo: implement generic parsing
            index = index + 1; //take this out and have the generics leave you on the word token
        }
        
        this.ReadToToken(readToOverride);
        
        return new TypeInfo(typeName);
    }
}