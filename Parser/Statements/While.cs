namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeWhileLoop(ParserState returnState)
    {
        state = ParserState.WHILE;

        Expression? condition = null; 
        this.ConsumeExpression2(ref condition);
        
        Statement.WhileStatement statement = new Statement.WhileStatement((Expression.BooleanOperator)condition);
        
        statement.Scope.returnNode = currentNode;
        statement.parent = currentNode;
        statement.Scope.returnState = returnState;
        statement.Scope.parent = currentNode.Scope;

        currentNode.children.Add(statement);

        currentNode = statement;
        index = index + 1;
    }
}