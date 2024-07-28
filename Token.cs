public class Token
{
    public TokenType type { get; set; }
    public KeywordType? keyword { get; set; } = null;

    public enum TokenType 
    {
        WORD,
        VALUE,
        SEMICOLON,
        COLON,
        LBRACE,
        RBRACE,
        LBRACK,
        RBRACK,
        LPAREN,
        RPAREN,
        COMMA,
        DOT,
        MINUS,
        PLUS,
        FSLASH,
        BSLASH,
        STAR,
        LALLIGATOR,
        RALLIGATOR,
        AMPERSAND,
        NEWLINE,
        WHITESPACE,
        NULL
    }

    public enum KeywordType
    {
        IF,
        FOR,
        WHILE,
        PRIVATE,
        STATIC,
        LET,
        REF,
        CONST,
        FROM,
        DO,
        STRUCT,
        SWITCH,
        RETURN,
        CONTINUE,
        EXTERN,
        NEW,
        BREAK,
        THROW,
        TRY,
        CATCH,
        THIS,
        USING
    }
    
    public static Dictionary<char, TokenType> tokenMatches = new Dictionary<char, TokenType>
    {
        { ';', TokenType.SEMICOLON },
        { ':', TokenType.COLON }
    };

    public static Dictionary<string, KeywordType> keywordMatches = new Dictionary<string, KeywordType>
    {

    };
}