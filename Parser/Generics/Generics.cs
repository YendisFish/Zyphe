namespace Zyphe.Parser;

public partial class Parser
{
    public List<Generic>? ConsumeGenerics()
    {
        List<Generic>? ret = new();

        Generic? current = null;
        for (bool readingGenerics = true; readingGenerics = true;)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.RALLIGATOR:
                {
                    ret.Add(current);
                    return ret;
                }
                case Token.TokenType.COLON:
                {
                    ret.Add(current);
                    index = index + 1;
                    break;
                }
                case Token.TokenType.WORD:
                {
                    if (/*(string)tokens[index].value == "where"*/ tokens[index].keyword == Token.KeywordType.WHERE)
                    {
                        index = index + 1;
                        current = current with
                        {
                            constraints = this.ConsumeConstraints() ?? throw new NullReferenceException()
                        };
                        //index = index + 1;

                        break;
                    }
                    
                    current = new Generic((string)tokens[index].value, null);
                    index = index + 1;

                    break;
                }
                default:
                {
                    current = new Generic((string)tokens[index].value, null);
                    index = index + 1;
                    
                    break;
                }
            }
        }
        
        return ret;
    }

    public List<Constraint>? ConsumeConstraints()
    {
        List<Constraint> constraints = new();
        for (bool readingCon = true; readingCon;)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.RALLIGATOR:
                {
                    return constraints;
                }
                case Token.TokenType.COLON:
                {
                    return constraints;
                }
                case Token.TokenType.COMMA:
                {
                    index = index + 1;
                    break;
                }
                default:
                {
                    Constraint.TypeConstraint constraint = new Constraint.TypeConstraint((string)tokens[index].value ?? throw new NullReferenceException());
                    constraints.Add(constraint);

                    index = index + 1;
                    
                    break;
                }
            }
        }
        
        return constraints;
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