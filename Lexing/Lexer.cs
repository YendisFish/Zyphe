public class Lexer
{
    //static char[] splitters = ";:\"\'{}()*&+=-<>., \n".ToCharArray();

    public static Token[] Tokenize(string file)
    {
        List<Token> ret = new();
        
        Token currentToken = new();
        string current = "";
        for(int i = 0; i < file.Length; i++)
        {
            if (file[i] == '\"')
            {
                currentToken.type = Token.TokenType.VALUE;
                currentToken.value = "";
                i = i + 1;
                for (; file[i] != '\"'; i++)
                {
                    currentToken.value = (string)currentToken.value + file[i];
                }

                i = i + 1;
                
                ret.Add(currentToken);
                currentToken = new();
            }
            
            if (file[i] == '\'')
            {
                currentToken.type = Token.TokenType.VALUE;
                currentToken.value = new char();
                i = i + 1;
                for (; file[i] != '\''; i++)
                {
                    currentToken.value = file[i];
                }

                i = i + 1;
                
                ret.Add(currentToken);
                currentToken = new();
            }
            
            //match delimiters to delimiter enum
            Token.TokenType type = new();
            if (Token.tokenMatches.TryGetValue(file[i], out type))
            {
                if (current != "")
                {
                    currentToken.type = Token.TokenType.WORD;
                    currentToken.value = current;
                    ret.Add(currentToken);

                    currentToken = new();
                    current = "";
                }
                
                currentToken.type = type;
                ret.Add(currentToken);
                currentToken = new();
            } else {
                current = current + file[i];
            }
            
            //match keywords to keyword enum
            Token.KeywordType keyword = new();
            if (Token.keywordMatches.TryGetValue(current, out keyword))
            {
                currentToken.type = Token.TokenType.WORD;
                currentToken.keyword = keyword;
                currentToken.value = current;
                ret.Add(currentToken);
                
                currentToken = new();
                current = "";
            }
        }

        return ret.ToArray();
    }

    public static Token[] RemoveWhitespace(Token[] toks) =>
        toks.Where(x => x.type != Token.TokenType.WHITESPACE &&
                        x.type != Token.TokenType.NEWLINE &&
                        x.type != Token.TokenType.NULL &&
                        (x.type == Token.TokenType.WORD && string.IsNullOrWhiteSpace((string?)x.value)) == false).ToArray();
}