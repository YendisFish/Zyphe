namespace Zyphe.Parser;

public partial class Parser
{
    public Expression? ConsumeExpression()
    {
        switch (tokens[index].type)
        {
            case Token.TokenType.WORD:
            {
                if(declaredVariables.Contains((string)tokens[index].value))
                {
                    // todo: implement variable references
                }

                if (tokens[index + 1].type == Token.TokenType.SEMICOLON)
                {
                    Expression.Literal literal = new Expression.Literal((string)tokens[index].value);
                    this.ReadToToken(Token.TokenType.SEMICOLON);
                    return literal;
                }
                
                break;
            }
        }
        
        this.ReadToToken(Token.TokenType.SEMICOLON);
        index = index + 1;
        return null;
    }
}