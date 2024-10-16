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
                currentToken.type = Token.TokenType.WORD;
                currentToken.value = "";
                i = i + 1;
                for (; file[i] != '\"'; i++)
                {
                    currentToken.value = (string)currentToken.value + file[i];
                }

                i = i + 1;

                currentToken.metadata = "string";
                
                ret.Add(currentToken);
                currentToken = new();
            }

            if (file[i] == '\'')
            {
                currentToken.type = Token.TokenType.WORD;
                currentToken.value = new char();
                i = i + 1;
                for (; file[i] != '\''; i++)
                {
                    currentToken.value = file[i];
                }

                i = i + 1;
                
                currentToken.metadata = "char";
                
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
                currentToken.value = file[i] + ""; //currentToken.value = current
                
                ret.Add(currentToken);
                currentToken = new();
            } else {
                current = current + file[i];
            }
            
            //match keywords to keyword enum
            Token.KeywordType keyword = new();
            if (Token.keywordMatches.TryGetValue(current, out keyword)/* && (!string.IsNullOrWhiteSpace("" + file[i + 1])) || !Token.tokenMatches.ContainsKey(file[i + 1])*/)
            {
                //this prevents the lexer from attempting to parse doSomething (includes "do") into 2 tokens
                if (i < file.Length - 1)
                {
                    if (!(string.IsNullOrWhiteSpace("" + file[i + 1])) && !Token.tokenMatches.ContainsKey(file[i + 1]))
                    {
                        continue;
                    }
                }
                
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