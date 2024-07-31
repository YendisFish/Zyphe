namespace Zyphe.Parser;

public class Parser
{
    public Token[] tokens { get; set; }
    public int index { get; set; } = 0;
    public AST ast { get; set; }
    
    public Parser(Token[] toks)
    {
        tokens = toks;
        ast = new();
    }
}