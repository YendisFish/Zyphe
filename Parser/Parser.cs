namespace Zyphe.Parser;

public partial class Parser
{
    public Token[] tokens { get; set; }
    public int index { get; set; } = 0;
    public AST ast { get; set; }
    public AstNode currentNode { get; set; }
    public ParserState state { get; set; } = ParserState.GLOBAL;
    public List<string> declaredVariables { get; set; } = new();
    public Statement.IfStatement? rootStatement { get; set; }
    public bool readingPrivateScope { get; set; } = false;

    public Parser(Token[] toks)
    {
        tokens = toks;
        ast = new();
        currentNode = ast.Root;
    }

    public void Parse()
    {
        while (index < tokens.Length)
        {
            //Console.WriteLine(tokens[index].type);
            switch (tokens[index].type)
            {
                case Token.TokenType.WORD:
                {
                    this.ConsumeWord();
                    break;
                }

                case Token.TokenType.RBRACE:
                {
                    switch (state)
                    {
                        case ParserState.FUNCTION:
                        case ParserState.STRUCT:
                        {
                            state = ParserState.GLOBAL;
                            currentNode = ast.Root;
                            this.Next();
                            break;
                        }

                        case ParserState.IF:
                        {
                            
                            if (!(index + 1 >= tokens.Length) && tokens[index + 1].keyword == Token.KeywordType.ELSE)
                            {
                                this.Next();
                            } else {
                                if (currentNode is Statement.IfStatement a &&
                                    a.Scope.returnNode is Statement.ElseStatement b &&
                                    b.isIfElse)
                                {
                                    currentNode = b.Scope.returnNode;
                                    //this.Next();
                                } else if (currentNode is Statement.ElseStatement)
                                {
                                    currentNode = currentNode.Scope.returnNode;
                                    //this.Next();
                                } else
                                {
                                    state = currentNode.Scope.returnState ?? throw new NullReferenceException();
                                    currentNode = currentNode.Scope.returnNode;
                                    this.Next();
                                }
                            }
                            
                            break;
                        }

                        case ParserState.WHILE:
                        {
                            currentNode = currentNode.Scope.returnNode;
                            this.Next();
                            break;
                        }

                        case ParserState.GETTER:
                        {
                            currentNode = currentNode.Scope.returnNode;
                            state = ParserState.PROP;
                            this.Next();
                            
                            break;
                        }
                            
                        case ParserState.SETTER:
                        {
                            currentNode = currentNode.Scope.returnNode;
                            state = ParserState.PROP;
                            break;
                        }
                        
                        case ParserState.PROP:
                        {
                            currentNode = currentNode.parent;
                            state = ParserState.STRUCT;
                            this.Next();
                            break;
                        }
                        
                        default:
                        {
                            this.Next();
                            break;
                        }
                    }
                    
                    break;
                }
                default:
                {
                    this.Next();
                    break;
                }
            }
        }
    }

    public void Next()
    {
        if (!(index + 1 > tokens.Length))
        {
            index = index + 1;
        }
    }

    public void ConsumeWord()
    {
        switch (tokens[index].keyword)
        {
            case Token.KeywordType.GET:
            {
                index = index + 1;
                this.ConsumeGetter();
                break;
            }
            
            case Token.KeywordType.SET:
            {
                index = index + 1;
                this.ConsumeSetter();
                break;
            }
                
            case Token.KeywordType.WHILE:
            {
                index = index + 2;
                this.ConsumeWhileLoop(state);
                break;
            }
                
            case Token.KeywordType.ELSE:
            {
                this.Next();
                this.ConsumeElse();
                break;
            }
                
            case Token.KeywordType.IF:
            {
                index = index + 2;

                bool isRoot = (state == ParserState.FUNCTION);
                this.ConsumeIfStatement(isRoot, state);
                
                break;
            }
            
            case Token.KeywordType.STRUCT:
            {
                index = index + 1;
                string name = (string)tokens[index].value;

                StructInfo sInfo = new StructInfo(name, this.ConsumeGenerics(), null);
                Declaration.StructDeclaration structDecl = new Declaration.StructDeclaration(sInfo);

                currentNode.children.Add(structDecl);
                currentNode = structDecl;

                state = ParserState.STRUCT;
                
                break;
            }
            case Token.KeywordType.PRIVATE:
            {
                #region Handle Functions

                index = index + 1;
                VariableIdentifier identifier = (tokens[index].keyword == Token.KeywordType.REF) ? VariableIdentifier.REF : VariableIdentifier.LET; 
                if(ast.Root.Scope.scopeId == currentNode.Scope.scopeId)
                {
                    this.ConsumeFunctionSignature(identifier, true);
                }
                
                #endregion

                break;
            }
            case Token.KeywordType.REF:
            {
                #region Handle Functions
                
                if(ast.Root.Scope.scopeId == currentNode.Scope.scopeId)
                {
                    this.ConsumeFunctionSignature(VariableIdentifier.REF);
                }
                
                #endregion
                
                #region Handle Variables 
                
                #endregion
                
                #region Handle Props
                
                #endregion
                
                break;
            }
            case Token.KeywordType.LET:
            {
                switch (state)
                {
                    case ParserState.FUNCTION:
                    case ParserState.IF:
                    case ParserState.ELSE:
                    case ParserState.WHILE:
                    case ParserState.GETTER:
                    case ParserState.SETTER:
                    {
                        index = index + 1;
                        this.ConsumeLetVariable(state);
                        break;
                    }

                    case ParserState.STRUCT:
                    {
                        index = index + 1;
                        this.ConsumeProp(VariableIdentifier.LET);
                        break;
                    }
                }
                break;
            }
            default:
            {
                if(ast.Root.Scope.scopeId == currentNode.Scope.scopeId)
                {
                    this.ConsumeFunctionSignature(VariableIdentifier.LET);
                }
                
                #region Consume Variable Expression
                
                // todo: implement variable expressions
                
                #endregion
                
                break;
            }
        }
    }
    
    public void ReadToToken(Token.TokenType tokenType)
    {
        if (tokens[index].type == tokenType)
        {
            index = index + 1;
        }
        
        while (tokens[index].type != tokenType)
        {
            index = index + 1;
        }
    }
    
    public void ReadToWord(Token.KeywordType keyword)
    {
        if (tokens[index].keyword == keyword)
        {
            index = index + 1;
        }
        
        while (tokens[index].keyword != keyword)
        {
            index = index + 1;
        }
    }
}

public enum ParserState
{
    FUNCTION,
    VARIABLE,
    STRUCT,
    PROP,
    GETTER,
    SETTER,
    TYPECAST,
    IF,
    ELSE,
    WHILE,
    GLOBAL
}