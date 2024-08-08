namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeIfStatement(bool isRoot, ParserState rState)
    {
        state = ParserState.IF;
        
        Expression? condition = null;
        this.ConsumeExpression2(ref condition);
        
        Statement.IfStatement statement = new Statement.IfStatement(condition);
        statement.isRoot = isRoot;
        statement.Scope.returnState = rState; //(currentNode is Statement.ElseStatement) ? ParserState.ELSE : ParserState.IF;
        
        currentNode.children.Add(statement);

        statement.parent = currentNode;
        statement.Scope.parent = currentNode.Scope;
        statement.Scope.returnNode = currentNode;
        
        currentNode = statement;
        index = index + 1;

        if (isRoot)
        {
            rootStatement = statement;
            statement.Scope.returnState = ParserState.FUNCTION;
        }
    }
    
    public void ConsumeElse()
    {
        Statement.IfStatement statement = (Statement.IfStatement)currentNode;
        Statement.ElseStatement elseBlock = new Statement.ElseStatement();
        
        if (tokens[index].keyword == Token.KeywordType.IF)
        {
            elseBlock.isIfElse = true;
        }
        
        //elseBlock.Scope = statement.Scope;
        //elseBlock.parent = statement;
        
        statement.elseBlock = elseBlock;
        
        elseBlock.Scope.returnNode = statement;
        currentNode = elseBlock;
    }
}