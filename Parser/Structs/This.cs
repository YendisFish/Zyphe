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
        } else if(tokens[index + 1].type == Token.TokenType.COLON) {
            index = index + 2;
            
            switch (tokens[index].type)
            {
                //here you will have to read delete, free, constructors, and other things
            }

            Statement.ThisStatement statement = new Statement.ThisStatement(ThisStatementType.Function);

            currentNode.children.Add(statement);

            statement.Scope.parent = currentNode.Scope;
            statement.parent = currentNode;
            statement.Scope.returnState = state;
            statement.Scope.returnNode = currentNode;
            
            currentNode = statement;
            
            state = ParserState.THIS;

            //index = index + 1;

            /*
             * +
             * -
             * /
             * *
             * delete
             * free
             * T[INDEX]
             * ClassName()
             * let T(ClassName right)
             * let ClassName(T right)
             */
        } else {
            throw new Exception("Invalid syntax used with \"this\" operator");
        }
    }
}