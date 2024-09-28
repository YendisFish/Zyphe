namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeLetVariable(ParserState returnState, bool isStatic = false)
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
                this.ConsumeExpression2(ref expr);
                index = index + 1;
                break;
            }

            case Token.TokenType.SEMICOLON:
            {
                index = index + 1;
                break;
            }
        }

        VariableInfo info = new VariableInfo(VariableIdentifier.LET, name, tInfo, false, readingPrivateScope, readingStaticVar);
        Declaration.VariableDeclaration declaration = new Declaration.VariableDeclaration(info, expr);

        declaration.Scope = currentNode.Scope;
        declaration.parent = currentNode;

        if (isStatic)
        {
            declaredGlobals.Add(name);
        }
        else
        {
            declaredVariables.Add(name);   
        }
        
        currentNode.children.Add(declaration);

        state = returnState;
        
        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
    }

    public void ConsumeRefVariable(ParserState returnState, bool isStatic = false)
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
                this.ConsumeExpression2(ref expr);
                index = index + 1;
                break;
            }

            case Token.TokenType.SEMICOLON:
            {
                index = index + 1;
                break;
            }
        }

        VariableInfo info = new VariableInfo(VariableIdentifier.REF, name, tInfo, false, readingPrivateScope, readingStaticVar);
        Declaration.VariableDeclaration declaration = new Declaration.VariableDeclaration(info, expr);

        declaration.Scope = currentNode.Scope;
        declaration.parent = currentNode;
        
        if (isStatic)
        {
            declaredGlobals.Add(name);
        }
        else
        {
            declaredVariables.Add(name);   
        }
        
        currentNode.children.Add(declaration);

        state = returnState;
        
        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
    }
    
    public TypeInfo ConsumeVarType(Token.TokenType readToOverride = Token.TokenType.EQUALS, bool skip = true)
    {
        string typeName = (string)tokens[index].value;
        List<GenericUsage> usages = null;
        
        //index = index + 1;
        
        if (tokens[index + 1].type == Token.TokenType.LALLIGATOR)
        {
            index = index + 1;
            usages = this.ConsumeGenericUsages() ?? throw new NullReferenceException();
        }

        index = index + 1;
        
        /*if (skip)
        {
            this.ReadToToken(readToOverride);
        }*/
        
        return new TypeInfo(typeName, usages);
    }

    public bool IsDeclared(string pattern)
    {
        return declaredGlobals.Contains(pattern) || 
               declaredProps.Contains(pattern) ||
               namespaces.Contains(pattern) ||
               declaredVariables.Contains(pattern);
    }
}