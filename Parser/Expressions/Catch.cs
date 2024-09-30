namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeCatch(ref Expression? expr)
    {
        index = index + 2;

        Expression? fref = null;
        this.ConsumeExpression2(ref fref);

        expr = new Expression.CatchExpression(fref as Expression.FunctionReference);

        expr.parent = currentNode;
        expr.Scope.parent = currentNode.Scope.parent;
        expr.Scope.returnNode = currentNode;
        expr.Scope.returnState = state;

        //currentNode.children.Add(expr);
        //currentNode = expr;
        
        state = ParserState.CATCH;
    }
}