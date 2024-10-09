namespace Zyphe.Parser;

public partial class Parser
{
    public List<Expression> ReadPassedArguments()
    {
        List<Expression> ret = new();
        
        //index = index + 1;

        for (bool readingArgs = true; readingArgs == true;)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.COMMA:
                {
                    index = index + 1;
                    
                    Expression? expr = null;
                    this.ConsumeExpression2(ref expr);

                    ret.Add(expr ?? throw new NullReferenceException());

                    break;
                }
                case Token.TokenType.RPAREN:
                {
                    readingArgs = false;
                    break;
                }
                default:
                {
                    Expression? expr = null;
                    this.ConsumeExpression2(ref expr);
                    
                    ret.Add(expr ?? throw new NullReferenceException());

                    break;
                }
            }   
        }

        return ret;
    }
}