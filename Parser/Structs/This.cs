namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeThisAccessor()
    {
        if (tokens[index + 1].type == Token.TokenType.DOT)
        {
            Expression? expr = null;
            this.ReadVariableAssignment(ref expr);

            expr.parent = currentNode;
            expr.Scope = currentNode.Scope;

            currentNode.children.Add(expr);
        } else if(tokens[index + 1].type == Token.TokenType.COLON) {
            index = index + 2;
            
            switch (tokens[index].type)
            {
                //here you will have to read delete, free, constructors, and other things

                case Token.TokenType.WORD:
                {
                    switch (tokens[index].keyword)
                    {
                        case Token.KeywordType.FREE:
                        {
                            Statement.ThisStatement statement = new Statement.ThisStatement(ThisStatementType.Free);

                            currentNode.children.Add(statement);

                            statement.Scope.parent = currentNode.Scope;
                            statement.parent = currentNode;
                            statement.Scope.returnState = state;
                            statement.Scope.returnNode = currentNode;
            
                            currentNode = statement;
            
                            state = ParserState.THIS;
                            
                            index = index + 1;

                            break;
                        }
                        case Token.KeywordType.DELETE:
                        {
                            Statement.ThisStatement statement = new Statement.ThisStatement(ThisStatementType.Delete);

                            currentNode.children.Add(statement);

                            statement.Scope.parent = currentNode.Scope;
                            statement.parent = currentNode;
                            statement.Scope.returnState = state;
                            statement.Scope.returnNode = currentNode;
            
                            currentNode = statement;
            
                            state = ParserState.THIS;

                            index = index + 1;
                            
                            break;
                        }

                        default:
                        {
                            if ((string)tokens[index].value == currentTypeName)
                            {
                                Statement.ThisStatement statement = new Statement.ThisStatement(ThisStatementType.Constructor);

                                index = index + 1;
                                List<VariableInfo> args = this.ParseArgs();

                                statement = statement with { arguments = args };
                        
                                currentNode.children.Add(statement);

                                statement.Scope.parent = currentNode.Scope;
                                statement.parent = currentNode;
                                statement.Scope.returnState = state;
                                statement.Scope.returnNode = currentNode;
            
                                currentNode = statement;
            
                                state = ParserState.THIS;

                                index = index + 1;
                            } else {
                                Statement.ThisStatement statement = new Statement.ThisStatement(ThisStatementType.Conversion);

                                index = index - 1; // this is the only time this parser will ever need to do lookback
                                VariableInfo inf = this.ReadVarInf() ?? throw new NullReferenceException();
                                currentConversionType = inf;
                        
                                currentNode.children.Add(statement);

                                statement.Scope.parent = currentNode.Scope;
                                statement.parent = currentNode;
                                statement.Scope.returnState = state;
                                statement.Scope.returnNode = currentNode;
            
                                currentNode = statement;
            
                                state = ParserState.THIS;
                            }

                            break;
                        }
                    }
                    break;
                }

                default:
                {
                    Statement.ThisStatement statement = new Statement.ThisStatement(ThisStatementType.Operator);

                    currentNode.children.Add(statement);

                    statement.Scope.parent = currentNode.Scope;
                    statement.parent = currentNode;
                    statement.Scope.returnState = state;
                    statement.Scope.returnNode = currentNode;
            
                    currentNode = statement;
            
                    state = ParserState.THIS;

                    break;
                }
            }
        } else {
            throw new Exception("Invalid syntax used with \"this\" operator");
        }
    }
    
    public void ConsumeThisGetter()
    {
        switch(tokens[index].type)
        {
            case Token.TokenType.LBRACE:
            {
                FunctionSignature signature = new FunctionSignature(new(currentConversionType.vType, currentConversionType.type),
                    currentConversionType.name + "_getter", false, new());
                Declaration.FunctionDeclaration func = new Declaration.FunctionDeclaration(signature);
                
                currentNode.children.Add(func);

                func.Scope.returnState = ParserState.THIS;
                func.Scope.returnNode = currentNode;
                currentNode = func;
                
                break;
            }
        }
        
        this.Next();
        
        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
        
        state = ParserState.GETTER;
    }
    
    public void ConsumeThisSetter()
    {
        switch(tokens[index].type)
        {
            
            case Token.TokenType.LBRACE:
            {
                FunctionSignature signature = new FunctionSignature(new(currentConversionType.vType, currentConversionType.type),
                    currentConversionType.name + "_setter", false, new()); //todo : Implement function arguments to cover setter inputs!
                Declaration.FunctionDeclaration func = new Declaration.FunctionDeclaration(signature);
                
                currentNode.children.Add(func);

                func.Scope.returnState = ParserState.THIS;
                func.Scope.returnNode = currentNode;
                currentNode = func;
                
                break;
            }
        }
        
        this.Next();
        
        state = ParserState.SETTER;
    }
}