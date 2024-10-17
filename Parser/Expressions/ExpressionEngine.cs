namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeExpression2(ref Expression? expr)
    {
        bool reading = true;
        while (reading)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.WORD:
                {
                    if (!this.CanBeIgnored((string)tokens[index].value) && 
                        (this.IsDeclared((string)tokens[index].value) ||
                        tokens[index + 1].type == Token.TokenType.LALLIGATOR &&
                        tokens[index + 2].type != Token.TokenType.SEMICOLON ||
                        tokens[index + 1].type == Token.TokenType.LPAREN)) //we might have a problemmmm
                    { 
                        this.ParseVar(ref expr);
                    } else {
                        // we can just return a literal right?
                        if (tokens[index].keyword == Token.KeywordType.NEW)
                        {
                            this.ParseNew(ref expr);
                            index = index + 1;
                        } else if (tokens[index].keyword == Token.KeywordType.CATCH) {
                            this.ConsumeCatch(ref expr);
                        } else if (tokens[index].keyword == Token.KeywordType.DELEGATE) {
                            this.ConsumeDelegate(ref expr);
                        } else if (tokens[index].keyword == Token.KeywordType.SIZEOF) {
                            index = index + 1;
                            
                            if(tokens[index].keyword == Token.KeywordType.STRUCT)
                            {
                                index = index + 1;
                                TypeInfo inf = this.ConsumeVarType();

                                expr = new Expression.SizeOf2(inf);
                            }
                            else
                            {
                                Expression? right = null;
                                this.ConsumeExpression2(ref right);

                                expr = new Expression.SizeOf(right);
                            }

                            //index = index + 1;
                        } else {
                            expr = new Expression.Literal((string)tokens[index].value, tokens[index].metadata);
                            index = index + 1;
                        }
                    }
                
                    break;
                }
                
                case Token.TokenType.COMMA:
                {
                    reading = false;
                    break;
                }

                case Token.TokenType.PLUS:
                case Token.TokenType.MINUS:
                case Token.TokenType.FSLASH:
                case Token.TokenType.STAR:
                {
                    Expression.BinaryOperator binOp = new Expression.BinaryOperator(expr, null, tokens[index].type);

                    index = index + 1;
                    
                    Expression? r = null;
                    this.ConsumeExpression2(ref r);
                    binOp.right = r;
                    
                    expr = binOp;
                    
                    break;
                }

                case Token.TokenType.LALLIGATOR:
                case Token.TokenType.RALLIGATOR:
                case Token.TokenType.PIPE:
                case Token.TokenType.NOT:
                case Token.TokenType.EQUALS:
                {
                    string operation = ResolveBooleanOperator(new[] { tokens[index].type, tokens[index + 1].type });
                    index = index + 2;
                    
                    Expression.BooleanOperator binOp = new Expression.BooleanOperator(expr, null, operation);
                    
                    Expression? r = null;
                    this.ConsumeExpression2(ref r);
                    binOp.right = r;
                    
                    expr = binOp;
                    
                    break;
                }

                case Token.TokenType.AMPERSAND:
                {
                    if (tokens[index + 1].type == Token.TokenType.AMPERSAND)
                    {
                        string operation = ResolveBooleanOperator(new[] { tokens[index].type, tokens[index + 1].type });
                        index = index + 2;
                    
                        Expression.BooleanOperator binOp = new Expression.BooleanOperator(expr, null, operation);
                    
                        Expression? r = null;
                        this.ConsumeExpression2(ref r);
                        binOp.right = r;
                    
                        expr = binOp;
                    } else {
                        index = index + 1;

                        Expression? r = null;
                        this.ConsumeExpression2(ref r);
                        
                        Expression.ReferenceOperator rop = new Expression.ReferenceOperator(r);

                        expr = rop;
                    }
                    
                    break;
                }

                case Token.TokenType.LPAREN:
                {
                    //expr = new Expression.BinaryOperator(null, null, null);
                    //Expression.BinaryOperator binOp = (Expression.BinaryOperator)expr;

                    index = index + 1;

                    if (tokens[index].keyword == Token.KeywordType.LET || tokens[index].keyword == Token.KeywordType.REF)
                    {
                        this.ParseTypeCast(ref expr);
                        //index = index + 1;
                    } else {
                        this.ConsumeExpression2(ref expr);
                        index = index + 1;    
                    }
                    
                    //this.ConsumeExpression2(binOp.left);
                    //this.ConsumeExpression2(binOp.right);
                    break;
                }

                case Token.TokenType.RPAREN:
                {
                    reading = false;
                    //index = index + 1;
                    
                    break;
                }

                case Token.TokenType.SEMICOLON:
                case Token.TokenType.RBRACK:
                {
                    reading = false;
                    //index = index + 1;
                    break;
                }
            }   
        }
    }

    public void ParseNew(ref Expression? expr)
    {
        index = index + 1;
        string name = (string)tokens[index].value!;

        index = index + 1;
        List<GenericUsage>? generics = this.ConsumeGenericUsages();
        index = index + 1;
        
        if (tokens[index].type == Token.TokenType.LBRACK)
        {
            //index = index + 1;
            Expression.IndexExpression? e = new Expression.IndexExpression();
            this.ReadIndex(ref e);
            
            //index = index + 1;
            
            expr = new Expression.NewArrayOperator(name, e, generics);
        } else {
            expr = new Expression.NewOperator(name, generics, null);
        
            List<Expression> args = this.ReadPassedArguments();
            expr = (expr as Expression.NewOperator ?? throw new NullReferenceException()) with { arguments = args };   
        }
        
        //this.ReadToToken(Token.TokenType.RPAREN); //REMOVE THIS?
    }

    public void ParseTypeCast(ref Expression? expr)
    {
        VariableIdentifier ident = (tokens[index].keyword == Token.KeywordType.LET || tokens[index].keyword == Token.KeywordType.REF) 
            ? VariableIdentifier.LET : VariableIdentifier.REF;
        
        index = index + 1;
        TypeInfo tinf = this.ConsumeVarType(false);
        
        if (tokens[index].type == Token.TokenType.LBRACK)
        {
            //index = index + 1;
            Expression.IndexExpression? e = new Expression.IndexExpression();
            this.ReadIndex(ref e);

            index = index + 2;
            Expression? r = null;
            this.ConsumeExpression2(ref r);

            expr = new Expression.TypeCastArray(ident, tinf, e, r);
        } else {
            index = index + 1;
            Expression? r = null;
            this.ConsumeExpression2(ref r);
            
            expr = new Expression.TypeCast(ident, tinf, r);
        }
    }
    
    public void ParseVar(ref Expression? expression)
    {
        string name = (string)tokens[index].value;
        Expression.IndexExpression ind = null;

        if ((tokens[index + 1].type == Token.TokenType.LALLIGATOR  && 
            tokens[index + 2].type != Token.TokenType.EQUALS ||
            tokens[index + 1].type == Token.TokenType.LPAREN) && !IsSkippedLiteral((string)tokens[index + 2].value))
        {
            expression = this.ConsumeFunctionCall();
        }
        else
        {
            index = index + 1;
            
            if (tokens[index].type == Token.TokenType.LBRACK)
            {
                ind = new Expression.IndexExpression();
                
                index = index + 1;
                this.ReadIndex(ref ind);
            }

            //take care of . to chain the expression!
            Expression.VariableReference rf = new Expression.VariableReference(name, null, null, null);
        
            switch (tokens[index].type)
            {
                case Token.TokenType.DOT:
                {
                    index = index + 1;
                
                    Expression? r = null;
                    this.ParseVar(ref r);
                
                    rf = rf with { chain = r };

                    try
                    {
                        r = (r as Expression.VariableReference) with { chainParent = rf };
                    } catch {
                        r = (r as Expression.FunctionReference) with { chainParent = rf };
                    }

                    //index = index + 1;
                
                    break;
                }
            }
            
            rf = rf with { index = ind };

            expression = rf;
        }
        
    }

    public void ReadIndex(ref Expression.IndexExpression expr)
    {
        for (bool reading = true; reading;)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.LBRACK:
                {
                    Expression.IndexExpression indC = new Expression.IndexExpression();

                    index = index + 1;
                    this.ReadIndex(ref indC);
                    expr = expr with { indexChain = indC };

                    //index = index + 1;

                    break;
                }
                case Token.TokenType.RBRACK:
                {
                    if (tokens[index + 1].type == Token.TokenType.LBRACK)
                    {
                        index = index + 1;
                    }
                    else
                    {
                        reading = false;
                    }
                    break;
                }
                default:
                {
                    Expression temp = new Expression.IndexExpression();
                    this.ConsumeExpression2(ref temp);

                    expr = expr with { expr = temp };

                    //index = index + 1;
                    
                    break;
                }
            }
        }
    }

    public string ResolveBooleanOperator(Token.TokenType[] tps)
    {
        string ret = "";

        foreach (var tType in tps)
        {
            switch (tType)
            {
                case Token.TokenType.AMPERSAND:
                {
                    ret = ret + "&";
                    break;
                }

                case Token.TokenType.PIPE:
                {
                    ret = ret + "|";
                    break;
                }

                case Token.TokenType.EQUALS:
                {
                    ret = ret + "=";
                    break;
                }

                case Token.TokenType.NOT:
                {
                    ret = ret + "!";
                    break;
                }

                case Token.TokenType.RALLIGATOR:
                {
                    ret = ret + ">";
                    break;
                }
                
                case Token.TokenType.LALLIGATOR:
                {
                    ret = ret + "<";
                    break;
                }
            }
        }

        return ret;
    }

    public void ReadVariableAssignment(ref Expression expr)
    {
        Expression? vref = null;
        this.ParseVar(ref vref);

        //index = index + 1;

        if (tokens[index].type == Token.TokenType.EQUALS)
        {
            Expression? assigment = null;
            this.ConsumeExpression2(ref assigment);

            expr = new Expression.VariableAssignment(vref as Expression.VariableReference, assigment);
            index = index + 1;
        } else {
            expr = vref;
        }
    }

    public bool IsSkippedLiteral(string value)
    {
        int y;
        bool i = int.TryParse(value, out y);

        byte a;
        bool b = byte.TryParse(value, out a);

        return i || b;
    }
}