namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeWhileLoop(ParserState returnState)
    {
        state = ParserState.WHILE;

        Expression condition = this.ConsumeExpression();
        Statement.WhileStatement statement = new Statement.WhileStatement((Expression.BooleanOperator)condition);
        statement.Scope.returnNode = currentNode;
        statement.Scope.returnState = returnState;
        
        statement.parent = currentNode;
        statement.Scope.parent = currentNode.Scope;

        currentNode.children.Add(statement);

        currentNode = statement;
        index = index + 1;
    }
}