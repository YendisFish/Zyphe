namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeSwitch()
    {
        index = index + 2;
        
        Expression? expr = null;
        this.ConsumeExpression2(ref expr);

        index = index + 1;

        Statement.SwitchStatement statement = new Statement.SwitchStatement(expr);
        
        currentNode.children.Add(statement);
        
        statement.Scope.returnNode = currentNode;
        statement.parent = currentNode;
        statement.Scope.returnState = state;
        statement.Scope.parent = currentNode.Scope;

        currentNode = statement;

        state = ParserState.SWITCH;
    }

    public void ConsumeCase()
    {
        index = index + 1;
        
        Expression? expr = new Expression.Literal((string)tokens[index].value, tokens[index].metadata);

        index = index + 3;
        
        Expression? expr2 = null;
        this.ConsumeExpression2(ref expr2);
        index = index + 1;

        Statement.CaseStatement statement = new Statement.CaseStatement(expr, expr2);
        
        currentNode.children.Add(statement);

        if (expr2 is Expression.Delegate)
        {
            currentNode = expr2;
        }
        
        //statement.Scope.returnNode = currentNode;
        //statement.parent = currentNode;
        //statement.Scope.returnState = state;
        //statement.Scope.parent = currentNode.Scope;
    }
}