namespace Zyphe.Parser;

public partial class Parser
{
    public List<Generic>? ConsumeGenerics()
    {
        index = index + 1;
        return null;
    }

    public List<GenericUsage>? ConsumeGenericUsages()
    {
        List<GenericUsage> ret = new();
        
        while (true)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.COMMA:
                case Token.TokenType.LALLIGATOR:
                {
                    index = index + 1;

                    if (tokens[index + 1].type == Token.TokenType.LALLIGATOR)
                    {
                        string name = (string)tokens[index].value;
                        index = index + 1;
                        TypeInfo tInf = new TypeInfo(name, this.ConsumeGenericUsages());

                        ret.Add(new(tInf));
                    }
                    else
                    {
                        string name = (string)tokens[index].value;
                        TypeInfo tInf = new TypeInfo(name, null);

                        index = index + 1;
                    
                        ret.Add(new(tInf));
                    }
                
                    break;
                }

                case Token.TokenType.RALLIGATOR:
                {
                    return ret;
                }

                default:
                {
                    return null;
                }
            }
        }

        return null;
    }
}