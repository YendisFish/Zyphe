using System.Linq.Expressions;

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
                    if (declaredVariables.Contains((string)tokens[index].value) || 
                        namespaces.Contains((string)tokens[index].value))
                    {
                        this.ParseVar(ref expr);
                    } else {
                        // we can just return a literal right?
                        if (tokens[index].keyword == Token.KeywordType.NEW)
                        {
                            this.ParseNew(ref expr);
                            index = index + 1;
                        } else {
                            expr = new Expression.Literal((string)tokens[index].value);
                            index = index + 1;
                        }
                    }
                
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
                    this.ConsumeExpression2(ref expr);
                    index = index + 1;
                    
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
        string name = (string)tokens[index].value;

        index = index + 1;
        List<GenericUsage>? generics = this.ConsumeGenericUsages();
        
        expr = new Expression.NewOperator(name, generics, null);
        
        //todo: IMPLEMENT ARGUMENT PARSING OMGGG!!!!!!!!
        
        this.ReadToToken(Token.TokenType.RPAREN);
    }

    public void ParseVar(ref Expression? expression)
    {
        string name = (string)tokens[index].value;
        Expression? ind = null;

        index = index + 1;

        if (tokens[index].type == Token.TokenType.LBRACK)
        {
            index = index + 1;
            this.ConsumeExpression2(ref ind); //take care of bracket exits here?
            index = index + 1;
        }

        //take care of . to chain the expression!
        Expression.VariableReference rf = new Expression.VariableReference(name, null, null, null);
        rf = rf with { index = ind };
        
        switch (tokens[index].type)
        {
            case Token.TokenType.DOT:
            {
                index = index + 1;
                
                Expression? r = null;
                this.ParseVar(ref r);
                
                rf = rf with { chain = r };
                r = (r as Expression.VariableReference) with { chainParent = rf };
                
                break;
            }
        }
        
        expression = rf;
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
            }
        }

        return ret;
    }
}