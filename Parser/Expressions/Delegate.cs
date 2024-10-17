namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeDelegate(ref Expression? expr)
    {
        //index = index + 1;

        List<VariableInfo> args = this.ParseArgs();

        expr = new Expression.Delegate(args);
        
        expr.parent = currentNode;
        expr.Scope.parent = currentNode.Scope.parent;
        expr.Scope.returnNode = currentNode;
        expr.Scope.returnState = ParserState.FUNCTION;
        
        state = ParserState.DELEGATE;
    }
}