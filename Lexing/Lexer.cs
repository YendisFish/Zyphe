public class Lexer
{
    //static char[] splitters = ";:\"\'{}()*&+=-<>., \n".ToCharArray();

    public static Token[] Tokenize(string file)
    {
        Token currentToken = new();
        string current = "";
        for(int i = 0; i < file.Length; i++)
        {
            //match delimeters to delimeter enum
            Token.TokenType type = new();
            if (Token.tokenMatches.TryGetValue(file[i], out type))
            {
                currentToken.type = type;
                
            } else {
                current = current + file[i];
            }

            //match keywords to keyword enum
            Token.KeywordType keyword = new();
            if (Token.keywordMatches.TryGetValue(current, out keyword))
            {
                currentToken.type = Token.TokenType.WORD;
                currentToken.keyword = keyword;

                current = "";
            }
        }

        return new Token[current.Length /*THIS WILL NOT WORKK!!!!!!*/];
    }
}