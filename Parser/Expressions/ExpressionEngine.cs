using System.Linq.Expressions;

namespace Zyphe.Parser;

public partial class Parser
{
    public Expression? ConsumeExpression(AstNode? parent = null)
    {
        switch (tokens[index].type)
        {
            case Token.TokenType.AMPERSAND:
            {
                index = index + 1;
                Expression.ReferenceOperator rOp = new Expression.ReferenceOperator(this.ConsumeExpression());
                rOp.parent = parent;
                rOp.children.Add(rOp.expr);
                rOp.expr.parent = rOp;
                return rOp;
            }
                
            case Token.TokenType.WORD:
            {
                switch (tokens[index + 1].type)
                {
                    //if the next token is a semicolon just parse the literal and return
                    case Token.TokenType.SEMICOLON:
                    {
                        Expression literal = ParseLiteralOrVarName();
                        literal.parent = parent;
                        this.ReadToToken(Token.TokenType.SEMICOLON);
                        return literal;
                    }

                    //if the next token is a right parenthesis then parse a literal, escape the parentheses and return
                    case Token.TokenType.RPAREN:
                    {
                        Expression literal = ParseLiteralOrVarName();
                        literal.parent = parent;
                        index = index + 2;
                        return literal;
                    }

                    //if the next token is a mathematical operator, parse a literal on the left, use recursive parsing on the right
                    case Token.TokenType.PLUS: 
                    case Token.TokenType.MINUS: 
                    case Token.TokenType.STAR: 
                    case Token.TokenType.FSLASH:
                    {
                        Expression literal = ParseLiteralOrVarName();
                        Token.TokenType operation = tokens[index + 1].type;
                        index = index + 2;
                        Expression.BinaryOperator expr = new Expression.BinaryOperator(literal, this.ConsumeExpression(), operation);
                        expr.parent = parent;
                        
                        literal.parent = expr;
                        expr.right.parent = expr;
                        
                        expr.children.Add(literal);
                        expr.children.Add(expr.right);
                        
                        return expr;
                    }

                    case Token.TokenType.AMPERSAND:
                    case Token.TokenType.PIPE:
                    case Token.TokenType.EQUALS:
                    case Token.TokenType.NOT:
                    {
                        Expression literal = ParseLiteralOrVarName();
                        string operation = ResolveBooleanOperator(new[] { tokens[index + 1].type, tokens[index + 2].type });
                        index = index + 3;
                        Expression.BooleanOperator expr = new Expression.BooleanOperator(literal, this.ConsumeExpression(), operation);
                        
                        expr.parent = parent;
                        
                        literal.parent = expr;
                        expr.right.parent = expr;
                        
                        expr.children.Add(literal);
                        expr.children.Add(expr.right);
                        
                        return expr;
                    }
                }
                
                break;
            }

            //if there is a left parenthesis, parse a literal, move to the side, and recursively parse
            case Token.TokenType.LPAREN:
            {
                index = index + 1;

                switch (tokens[index + 1].type)
                {
                    case Token.TokenType.AMPERSAND:
                    case Token.TokenType.PIPE:
                    case Token.TokenType.EQUALS:
                    case Token.TokenType.NOT:
                    {
                        Expression literal = ParseLiteralOrVarName();
                        string operation = ResolveBooleanOperator(new[] { tokens[index + 1].type, tokens[index + 2].type });
                        index = index + 3;
                        Expression.BooleanOperator expr = new Expression.BooleanOperator(literal, this.ConsumeExpression(), operation);
                        
                        expr.parent = parent;
                        
                        literal.parent = expr;
                        expr.right.parent = expr;
                        
                        expr.children.Add(literal);
                        expr.children.Add(expr.right);
                        
                        return expr;
                    }

                    default:
                    {
                        Expression literal = ParseLiteralOrVarName();
                        Token.TokenType operation = tokens[index + 1].type;

                        index = index + 2;
                
                        Expression.BinaryOperator expr = new Expression.BinaryOperator(literal, this.ConsumeExpression(), operation);
                        
                        expr.parent = parent;
                        
                        literal.parent = expr;
                        expr.right.parent = expr;
                        
                        expr.children.Add(literal);
                        expr.children.Add(expr.right);
                        
                        return expr;
                    }
                }
            }
        }

        return null;
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

    public Expression ParseLiteralOrVarName() => (declaredVariables.Contains(tokens[index].value)) || //check if a defined variable has the symbol name
                                                  (state == ParserState.SETTER && (string)tokens[index].value == "value") //check if we are reading a setter value
        ? new Expression.VariableReference((string)tokens[index].value)
        : new Expression.Literal((string)tokens[index].value);
}