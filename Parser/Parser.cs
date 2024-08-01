namespace Zyphe.Parser;

public partial class Parser
{
    public Token[] tokens { get; set; }
    public int index { get; set; } = 0;
    public AST ast { get; set; }
    public AstNode currentNode { get; set; }

    public Parser(Token[] toks)
    {
        tokens = toks;
        ast = new();
        currentNode = ast.Root;
    }

    public void Parse()
    {
        while (index <= tokens.Length)
        {
            switch (tokens[index].type)
            {
                case Token.TokenType.WORD:
                {
                    this.ConsumeWord();
                    break;
                }
            }
            
            index = index + 1;
        }
    }

    public void Next() => index++;

    public void ConsumeWord()
    {
        switch (tokens[index].keyword)
        {
            case Token.KeywordType.PRIVATE:
            {
                #region Handle Functions

                index = index + 1;
                VariableIdentifier identifier = (tokens[index].keyword == Token.KeywordType.REF) ? VariableIdentifier.REF : VariableIdentifier.LET; 
                if(ast.Root.Scope.scopeId == currentNode.Scope.scopeId)
                {
                    this.ConsumeFunctionSignature(identifier, true);
                }
                
                #endregion

                break;
            }
            case Token.KeywordType.REF:
            {
                #region Handle Functions
                
                if(ast.Root.Scope.scopeId == currentNode.Scope.scopeId)
                {
                    this.ConsumeFunctionSignature(VariableIdentifier.REF);
                }
                
                #endregion
                
                break;
            }
            default:
            {
                if(ast.Root.Scope.scopeId == currentNode.Scope.scopeId)
                {
                    this.ConsumeFunctionSignature(VariableIdentifier.LET);
                }
                
                break;
            }
        }
    }
    
    public void ReadToToken(Token.TokenType tokenType)
    {
        if (tokens[index].type == tokenType)
        {
            index = index + 1;
        }
        
        while (tokens[index].type != tokenType)
        {
            index = index + 1;
        }
    }
}