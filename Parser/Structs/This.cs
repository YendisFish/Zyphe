namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeThisAccessor()
    {
        if (tokens[index + 1].type == Token.TokenType.DOT)
        {
            Expression? expr = null;
            this.ReadVariableAssignment(ref expr);

            expr.parent = currentNode;
            expr.Scope = currentNode.Scope;

            currentNode.children.Add(expr);
        } else if(tokens[index + 1].type == Token.TokenType.SEMICOLON) {
            // todo : implement this overloads
            
            /*
             * +
             * -
             * /
             * *
             * delete
             * free
             * T[INDEX]
             * T()
             */
        } else {
            throw new Exception("Invalid syntax used with \"this\" operator");
        }
    }
}