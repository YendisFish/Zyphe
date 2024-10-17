namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeLetVariable(ParserState returnState, bool isStatic = false)
    {
        //state = ParserState.VARIABLE;
        
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
                
                if (tokens[index].keyword == Token.KeywordType.CATCH)
                {
                    this.ConsumeCatch(ref expr);
                    index = index + 1;
                } else {
                    this.ConsumeExpression2(ref expr);
                    index = index + 1;    
                }
                
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
        declaration.Scope.parent = currentNode.Scope;
        declaration.Scope.returnNode = currentNode;
        //declaration.Scope.returnState = state; //WHY IS THIS LINE DOING WHAT IT IS DOING?!?!?!?!?!?
        declaration.parent = currentNode;
        
        currentNode.children.Add(declaration);

        if (isStatic)
        {
            declared.Globals.Add(declaration);
            //declaredGlobals.Add(name);
        }
        else
        {
            declared.Variables.Add(declaration);
            //declaredVariables.Add(declaration);
        }

        if (expr is Expression.CatchExpression)
        {
            currentNode = expr;
        }
        
        if (expr is Expression.Delegate)
        {
            currentNode = expr;
        }
        
        //state = returnState;
        
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
        declaration.Scope.parent = currentNode.Scope;
        declaration.Scope.returnNode = currentNode;
        //declaration.Scope.returnState = state;
        declaration.parent = currentNode;
        
        currentNode.children.Add(declaration);
        
        if (isStatic)
        {
            declared.Globals.Add(declaration);
            //declaredGlobals.Add(name);
        }
        else
        {
            declared.Variables.Add(declaration);
            //declaredVariables.Add(declaration);   
        }
        
        if (expr is Expression.CatchExpression)
        {
            currentNode = expr;
        }

        if (expr is Expression.Delegate)
        {
            currentNode = expr;
        }

        //state = returnState;
        
        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
    }
    
    public TypeInfo ConsumeVarType(bool readArray = true)
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

        index = index + 1;
        
        //check for array
        if (tokens[index].type == Token.TokenType.LBRACK && readArray)
        {
            index = index + 1;
            isArray = true;
            index = index + 1;
        }
        
        return new TypeInfo(typeName, usages, isArray);
    }

    public bool IsDeclared(string pattern, bool varsOnly = false) => (varsOnly)
        ? declared.Variables.Select(x => x.left.name).Contains(pattern) ||
          declared.Arguments.Select(x => x.name).Contains(pattern) ||
          declared.Globals.Select(x => x.left.name).Contains(pattern) ||
          declared.Props.Select(x => x.left.name).Contains(pattern)
        : declared.Variables.Select(x => x.left.name).Contains(pattern) ||
          declared.Arguments.Select(x => x.name).Contains(pattern) ||
          declared.Globals.Select(x => x.left.name).Contains(pattern) ||
          declared.Props.Select(x => x.left.name).Contains(pattern) ||
          declared.Funcs.Select(x => x.signature.name).Contains(pattern) ||
          declared.GlobalFuncs.Select(x => x.signature.name).Contains(pattern);
}