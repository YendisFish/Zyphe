namespace Zyphe.Parser;

public partial class Parser
{
    public Expression.FunctionReference ConsumeFunctionCall()
    {
        Expression.FunctionReference rf = new Expression.FunctionReference("", null, null, null, null);
        
        string name = (string)tokens[index].value;
        List<GenericUsage> generics = null;
        index = index + 1;
        
        if (tokens[index].type == Token.TokenType.LALLIGATOR)
        {
            generics = this.ConsumeGenericUsages() ?? throw new NullReferenceException();
            index = index + 1;
        }

        index = index + 1;
        List<Expression> arguments = this.ReadPassedArguments();
        //index = index + 1;

        Expression? ind = null;
        
        if (tokens[index].type == Token.TokenType.LBRACK)
        {
            index = index + 1;
            this.ConsumeExpression2(ref ind); //take care of bracket exits here?
            index = index + 1;
        }

        //take care of . to chain the expression!
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

        return rf with { name = name, arguments = arguments, generics = generics, index = ind };
    }
}