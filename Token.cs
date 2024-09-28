public class Token
{
    public TokenType type { get; set; }
    public KeywordType? keyword { get; set; } = null;
    public object? value { get; set; } = null;
    public bool isString { get; set; } = false;
    public bool isChar { get; set; } = false;

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
        EQUALS,
        FSLASH,
        BSLASH,
        STAR,
        LALLIGATOR,
        RALLIGATOR,
        AMPERSAND,
        NEWLINE,
        WHITESPACE,
        NULL,
        PIPE,
        NOT
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
        USING,
        DELEGATE,
        ABSTRACT,
        ELSE,
        GET,
        SET,
        ENUM,
        WHERE
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
        { '=', TokenType.EQUALS },
        { '/', TokenType.FSLASH },
        { '\\', TokenType.BSLASH },
        { '*', TokenType.STAR },
        { '<', TokenType.LALLIGATOR },
        { '>', TokenType.RALLIGATOR },
        { '&', TokenType.AMPERSAND },
        { '\n', TokenType.NEWLINE },
        { ' ', TokenType.WHITESPACE },
        { '\0', TokenType.NULL },
        { '|', TokenType.PIPE },
        { '!', TokenType.NOT }
    };

    public static Dictionary<string, KeywordType> keywordMatches = new Dictionary<string, KeywordType>
    {
        { "if", KeywordType.IF },
        { "for", KeywordType.FOR },
        { "while", KeywordType.WHILE },
        { "private", KeywordType.PRIVATE },
        { "static", KeywordType.STATIC },
        { "let", KeywordType.LET },
        { "ref", KeywordType.REF },
        { "const", KeywordType.CONST },
        { "from", KeywordType.FROM },
        { "struct", KeywordType.STRUCT },
        { "switch", KeywordType.SWITCH },
        { "return", KeywordType.RETURN },
        { "continue", KeywordType.CONTINUE },
        { "extern", KeywordType.EXTERN },
        { "new", KeywordType.NEW },
        { "break", KeywordType.BREAK },
        { "throw", KeywordType.THROW },
        { "catch", KeywordType.CATCH },
        { "this", KeywordType.THIS },
        { "using", KeywordType.USING },
        { "delegate", KeywordType.DELEGATE },
        { "abstract", KeywordType.ABSTRACT },
        { "else", KeywordType.ELSE },
        { "get", KeywordType.GET },
        { "set", KeywordType.SET },
        { "where", KeywordType.WHERE }
    };
}