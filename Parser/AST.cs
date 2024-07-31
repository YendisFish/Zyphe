namespace Zyphe.Parser;

public class AST
{
    public AstNode Root { get; set; }
}

public abstract record AstNode
{
    public Scope Scope { get; init; }
    public AstNode children { get; set; }
}

public record Expression() : AstNode
{
    public record BinaryOperator(Expression left, Expression right) : Expression;
    public record UnaryOperator(Expression expr) : Expression;
    public record VariableExpression(string name, Expression right) : Expression;
}

public abstract record Declaration(Namespace? nspace) : AstNode
{
    public Declaration() : this(nspace: null) { }

    public record VariableDeclaration(VariableInfo left, Expression right) : Declaration;
    public record FunctionDeclaration(FunctionSignature signature) : Declaration;
    public record StructDeclaration(StructInfo info) : Declaration;
}

// todo : IMPLEMENT STATEMENT NODES

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