namespace Zyphe.Parser;

public partial class Parser
{
    public Token[] tokens { get; set; }
    public int index { get; set; } = 0;
    public AST ast { get; set; }
    public AstNode currentNode { get; set; }
    public ParserState state { get; set; } = ParserState.GLOBAL;
    public List<string> declaredVariables { get; set; } = new();
    public List<string> declaredGlobals { get; set; } = new();
    public List<string> declaredProps { get; set; } = new();
    public List<string> namespaces { get; set; } = new();
    public Statement.IfStatement? rootStatement { get; set; }
    public bool readingPrivateScope { get; set; } = false;
    public bool readingStaticVar { get; set; } = false;

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
                        case ParserState.FOR:
                        {
                            state = currentNode.Scope.returnState ?? throw new NullReferenceException();
                            currentNode = currentNode.parent ?? throw new NullReferenceException();
                            
                            this.Next();
                            
                            break;
                        }
                        case ParserState.FUNCTION:
                        {
                            if ((currentNode as Declaration.FunctionDeclaration).isStructFunc)
                            {
                                state = ParserState.STRUCT;
                                currentNode = currentNode.parent;
                                this.Next();
                            }
                            else
                            {
                                state = ParserState.GLOBAL;
                                currentNode = ast.Root;
                                this.Next();
                            }
                            
                            declaredVariables = new();

                            break;
                        }
                        case ParserState.STRUCT:
                        {
                            state = ParserState.GLOBAL;
                            currentNode = ast.Root;
                            this.Next();
                            
                            declaredProps = new();
                            
                            break;
                        }

                        case ParserState.IF:
                        {

                            if (!(index + 1 >= tokens.Length) && tokens[index + 1].keyword == Token.KeywordType.ELSE)
                            {
                                this.Next();
                            }
                            else
                            {
                                if (currentNode is Statement.IfStatement a &&
                                    a.Scope.returnNode is Statement.ElseStatement b &&
                                    b.isIfElse)
                                {
                                    currentNode = b.Scope.returnNode;
                                    //this.Next();
                                }
                                else if (currentNode is Statement.ElseStatement)
                                {
                                    currentNode = currentNode.Scope.returnNode;
                                    //this.Next();
                                }
                                else
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

                            declaredVariables = new();
                            
                            break;
                        }

                        case ParserState.SETTER:
                        {
                            currentNode = currentNode.Scope.returnNode;
                            state = ParserState.PROP;

                            if (tokens[index + 1].type == Token.TokenType.RBRACE)
                            {
                                this.Next();
                            }

                            declaredVariables = new();
                            
                            break;
                        }

                        case ParserState.PROP:
                        {
                            currentNode = currentNode.Scope.returnNode;
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
            case Token.KeywordType.FOR:
            {
                index = index + 1;
                this.ConsumeForLoop(state);

                break;
            }
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
                this.ConsumeStruct();
                index = index + 1;
                
                break;
            }
            case Token.KeywordType.PRIVATE:
            {
                index = index + 1;
                readingPrivateScope = true;

                break;
            }
            case Token.KeywordType.STATIC:
            {
                index = index + 1;
                readingStaticVar = true;

                switch (tokens[index].keyword)
                {
                    case Token.KeywordType.REF:
                    {
                        index = index + 1;
                        this.ConsumeRefVariable(ParserState.GLOBAL);
                        break;
                    }
                    case Token.KeywordType.LET:
                    {
                        index = index + 1;
                        this.ConsumeLetVariable(ParserState.GLOBAL);
                        break;
                    }
                }
                
                readingStaticVar = false;

                break;
            }
            case Token.KeywordType.REF:
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
                        this.ConsumeRefVariable(state);
                        break;
                    }

                    case ParserState.STRUCT:
                    {
                        index = index + 1;

                        //identify var or func
                        int currentIndex = index;
                        bool isFunc = false;

                        switch (tokens[index + 1].type)
                        {
                            case Token.TokenType.LALLIGATOR:
                            {
                                isFunc = true;
                                break;
                            }
                            default:
                            {
                                if (tokens[index + 2].type == Token.TokenType.LALLIGATOR || tokens[index + 2].type == Token.TokenType.LPAREN)
                                {
                                    isFunc = true;
                                }

                                break;
                            }
                        }

                        if (isFunc)
                        {
                            this.ConsumeFunctionSignature(VariableIdentifier.REF, true);
                        }
                        else
                        {
                            this.ConsumeProp(VariableIdentifier.REF);
                        }

                        break;
                    }

                    case ParserState.GLOBAL:
                    {
                        this.ConsumeFunctionSignature(VariableIdentifier.REF, false);
                        break;
                    }
                }

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
            case Token.KeywordType.USING:
            {
                index = index + 1;
                Statement.UsingStatement statement = new Statement.UsingStatement((string)tokens[index].value);
                
                ast.Root.children.Add(statement);
                index = index + 1;

                namespaces.Add(statement.module);

                break;
            }
            default:
            {
                if (ast.Root.Scope.scopeId == currentNode.Scope.scopeId)
                {
                    this.ConsumeFunctionSignature(VariableIdentifier.LET, false);
                }

                //identify a function in a struct and parse it 
                if ((tokens[index + 2].type == Token.TokenType.LALLIGATOR ||
                    tokens[index + 2].type == Token.TokenType.LPAREN) && state == ParserState.STRUCT)
                {
                    this.ConsumeFunctionSignature(VariableIdentifier.LET, true);
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
    GLOBAL,
    FOR
}
