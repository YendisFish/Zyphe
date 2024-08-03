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
    public record BinaryOperator(Expression left, Expression right, Token.TokenType operation) : Expression;
    public record UnaryOperator(Expression expr) : Expression;
    public record VariableAssignment(string name, Expression right) : Expression;
    public record VariableReference(string name) : Expression;
    public record FunctionReference(string name) : Expression;
    public record Literal(string word) : Expression; //this can be string, any integer type, or a boolean... parser will have to figure that out
    public record BooleanOperator(Expression left, Expression right, string operation) : Expression;
    public record ReferenceOperator(Expression expr) : Expression;
}

public abstract record Declaration(Namespace? nspace) : AstNode
{
    public Declaration() : this(nspace: null) { }

    public record VariableDeclaration(VariableInfo left, Expression? initializer) : Declaration;
    public record FunctionDeclaration(FunctionSignature signature) : Declaration;
    public record StructDeclaration(StructInfo info) : Declaration;
}

public abstract record Statement() : AstNode
{
    public record IfStatement(Expression condition) : Statement
    {
        public ElseStatement? elseBlock { get; set; } = null;
        public bool isRoot { get; set; } = false;
    }

    public record ElseStatement() : Statement
    {
        public bool isIfElse { get; set; } = false;
    }
    
    public record WhileStatement(Expression.BooleanOperator condition) : Statement;

    public record ForStatement(
        Declaration.VariableDeclaration? declaration,
        Expression.BooleanOperator? condition,
        Expression expression) : Statement;

    public record FromStatement(Expression.FunctionReference func, List<Case> cases, Default? fallback = null) : Statement;

    public record TryCatchStatement(
        Expression.FunctionReference target,
        Declaration.VariableDeclaration catchVar,
        AstNode tblock,
        AstNode cblock) : Statement;

    public record SwitchStatement(Expression.VariableReference reference, List<Case> cases, Default? fallback = null) : Statement;
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