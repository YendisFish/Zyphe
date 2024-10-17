namespace Zyphe.Parser;

public class AST
{
    public AstNode Root { get; set; } = new AstNode();
}
    
public record AstNode
{
    public AstNode? parent { get; set; }
    public Scope Scope { get; set; }
    public List<AstNode> children { get; set; } = new();

    public AstNode()
    {
        parent = null;
        Scope = new();
    }
}

public record Expression() : AstNode
{
    public record Delegate(List<VariableInfo> argument) : Expression;
    public record TypeCast(VariableIdentifier ident, TypeInfo tinf, Expression? right) : Expression;
    public record TypeCastArray(VariableIdentifier ident, TypeInfo tinf, Expression length, Expression? right) : Expression;
    public record CatchExpression(FunctionReference func) : Expression;
    public record FreeExpression(Expression subject) : Expression;
    public record DeleteExpression(VariableReference subject) : Expression;
    
    public record BinaryOperator : Expression
    {
        public Expression? left { get; set; }
        public Expression? right { get; set; }
        public Token.TokenType type { get; set; }

        public BinaryOperator(Expression? l, Expression? r, Token.TokenType t)
        {
            left = l;
            right = r;
            type = t;
        }
    }
    public record UnaryOperator(Expression expr) : Expression;
    public record VariableAssignment(VariableReference name, Expression right) : Expression;
    public record NewOperator(
        string name, 
        List<GenericUsage>? generics = null, 
        List<Expression>? arguments = null) : Expression;
    public record NewArrayOperator(
        string name,
        IndexExpression expr,
        List<GenericUsage>? generics = null) : Expression;
    public record VariableReference(string name, IndexExpression? index, Expression? chain, Expression? chainParent)
        : Expression;
    public record FunctionReference(
        string name, 
        IndexExpression? index, 
        Expression? chain, 
        Expression? chainParent,
        List<GenericUsage>? generics = null,
        List<Expression>? arguments = null) : Expression;
    public record Literal(string word, string assumedType) : Expression; //this can be string, any integer type, or a boolean... parser will have to figure that out

    public record BooleanOperator : Expression
    {
        public Expression? left { get; set; }
        public Expression? right { get; set; }
        public string type { get; set; }

        public BooleanOperator(Expression? l, Expression? r, string t)
        {
            left = l;
            right = r;
            type = t;
        }
    }

    public record IndexExpression : Expression
    {
        public IndexExpression? indexChain { get; init; }
        public Expression expr { get; init; }
    }
    
    public record ReferenceOperator(Expression expr) : Expression;
}

public abstract record Declaration(Namespace? nspace) : AstNode
{
    public Declaration() : this(nspace: null) { }

    public record VariableDeclaration(VariableInfo left, Expression? initializer) : Declaration;

    public record FunctionDeclaration(FunctionSignature signature) : Declaration
    {
        public bool isStructFunc { get; set; } = false;
    }
    public record StructDeclaration(StructInfo info) : Declaration;
}

public abstract record Statement() : AstNode
{
    public record ReturnStatement(Expression expr) : Statement;
    public record ThisStatement(ThisStatementType type, List<VariableInfo>? arguments = null) : Statement;
    
    public record IfStatement(Expression condition) : Statement
    {
        public ElseStatement? elseBlock { get; set; } = null;
        public bool isRoot { get; set; } = false;
    }

    public record ElseStatement() : Statement
    {
        public bool isIfElse { get; set; } = false;
    }

    public record UsingStatement(string module) : Statement;
    
    public record WhileStatement(Expression.BooleanOperator condition) : Statement;

    public record ForStatement(
        Declaration.VariableDeclaration? declaration,
        Expression? condition,
        Expression expression) : Statement;
    
    public record ForStatement2(Declaration.VariableDeclaration? declaration) : Statement;

    public record SwitchStatement(Expression expression) : Statement;
    public record CaseStatement(Expression expression, Expression right) : Statement;
    public record DefaultStatement(Expression expression, Expression right) : Statement;
}

public enum ThisStatementType
{
    Delete,
    Free,
    Constructor,
    Conversion,
    Indexer,
    Operator
}

/*
    void Main() {
        io.conout("Hello, World!");
        
        let x = 5;
        if(x == 5) {
            ref x2: int = &x;
            
            io.conout((let int)&x); //printing out the addr
        }
    }
*/