namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeLetVariable()
    {
        string name = (string)tokens[index].value;
        TypeInfo? tInfo = null;
        Expression? expr = null;
        index = index + 1;

        switch (tokens[index].type)
        {
            case Token.TokenType.COLON:
            {
                index = index + 1;
                tInfo = this.ConsumeType();
                break;
            }

            case Token.TokenType.EQUALS:
            {
                index = index + 1;
                expr = this.ConsumeExpression();
                break;
            }

            case Token.TokenType.SEMICOLON:
            {
                index = index + 1;
                break;
            }
        }

        if (tInfo is null)
        {
            // todo : type identification engine, little bit of semantics fr
        }

        VariableInfo info = new VariableInfo(VariableIdentifier.LET, name, tInfo, null, null);
        Declaration.VariableDeclaration declaration = new Declaration.VariableDeclaration(info, expr);

        declaredVariables.Add(name);
        currentNode.children.Add(declaration);

        state = ParserState.FUNCTION;
    }
}