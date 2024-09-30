namespace Zyphe.Parser;

public partial class Parser
{
    public List<VariableInfo> ParseArgs()
    {
        List<VariableInfo> ret = new();

        index = index + 1;

        if (tokens[index + 1].type == Token.TokenType.RPAREN)
        {
            return ret;
        }
        
        for (bool reading = true; reading;)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.LPAREN:
                case Token.TokenType.COMMA:
                {
                    VariableInfo? vInfo = this.ReadVarInf();
                    ret.Add(vInfo);
                    index = index + 1;
                    break;
                }

                case Token.TokenType.RPAREN:
                { 
                    //index = index + 1; //might not wanna do this
                    reading = false;
                    break;
                }
            }
        }

        return ret;
    }

    public VariableInfo? ReadVarInf()
    {
        index = index + 1;
        Token.KeywordType? kTp = tokens[index].keyword;

        VariableIdentifier vIden = (kTp == Token.KeywordType.LET) ? VariableIdentifier.LET : VariableIdentifier.REF;

        index = index + 1;
        string name = (string)tokens[index].value;

        index = index + 1;
        TypeInfo tpInf = this.ConsumeType();

        return new VariableInfo(vIden, name, tpInf);

        //todo: actually implement this
    }
}
