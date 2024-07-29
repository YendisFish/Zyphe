public class Token
{
    public TokenType type { get; set; }
    public KeywordType? keyword { get; set; } = null;
    public object? value { get; set; } = null;

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
        { ':', TokenType.COLON },
        { '{', TokenType.LBRACE },
        { '}', TokenType.RBRACE },
        { '[', TokenType.LBRACK },
        { ']', TokenType.RBRACK },
        { '(', TokenType.LPAREN },
        { ')', TokenType.RPAREN },
        { ',', TokenType.COMMA },
        { '.', TokenType.DOT },
        { '-', TokenType.MINUS },
        { '+', TokenType.PLUS },
        { '/', TokenType.FSLASH },
        { '\\', TokenType.BSLASH },
        { '*', TokenType.STAR },
        { '<', TokenType.LALLIGATOR },
        { '>', TokenType.RALLIGATOR },
        { '&', TokenType.AMPERSAND },
        { '\n', TokenType.NEWLINE },
        { ' ', TokenType.WHITESPACE },
        { '\0', TokenType.NULL },
    };

    public static Dictionary<string, KeywordType> keywordMatches = new Dictionary<string, KeywordType>
    {
        { "if", KeywordType.IF },
        { "FOR", KeywordType.FOR },
        { "WHILE", KeywordType.WHILE },
        { "private", KeywordType.PRIVATE },
        { "static", KeywordType.STATIC },
        { "let", KeywordType.LET },
        { "ref", KeywordType.REF },
        { "const", KeywordType.CONST },
        { "from", KeywordType.FROM },
        { "do", KeywordType.DO },
        { "struct", KeywordType.STRUCT },
        { "switch", KeywordType.SWITCH },
        { "return", KeywordType.RETURN },
        { "continue", KeywordType.CONTINUE },
        { "extern", KeywordType.EXTERN },
        { "new", KeywordType.NEW },
        { "break", KeywordType.BREAK },
        { "throw", KeywordType.THROW },
        { "try", KeywordType.TRY },
        { "catch", KeywordType.CATCH },
        { "this", KeywordType.THIS },
        { "using", KeywordType.USING },
    };
}