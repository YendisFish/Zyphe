namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeDelegate(ref Expression? expr)
    {
        //index = index + 1;

        if (tokens[index + 1].type != Token.TokenType.LPAREN)
        {
            index = index + 1;
            VariableInfo inf = new VariableInfo(VariableIdentifier.LET, (string)tokens[index].value);
            
            Declaration.VariableDeclaration fakeInstance = new Declaration.VariableDeclaration(inf, null);
            declared.Variables.Add(fakeInstance);

            expr = new Expression.Delegate(new List<VariableInfo>() { inf });
            
            expr.parent = currentNode;
            expr.Scope.parent = currentNode.Scope.parent;
            expr.Scope.returnNode = currentNode;
            expr.Scope.returnState = state;

            index = index + 3;

            Expression? right = null;
            this.ConsumeExpression2(ref right);
            
            expr.children.Add(new Statement.ReturnStatement(right));
            
            declared.Variables.Remove(fakeInstance);
        } else {
            List<VariableInfo> args = this.ParseArgs();

            expr = new Expression.Delegate(args);
        
            expr.parent = currentNode;
            expr.Scope.parent = currentNode.Scope.parent;
            expr.Scope.returnNode = currentNode;
            expr.Scope.returnState = state;
        
            state = ParserState.DELEGATE;   
        }

        //index = index + 1;
    }
}