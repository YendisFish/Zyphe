namespace Zyphe.Parser;

public record Case(Expression expression, AstNode right);
public record Default(AstNode right);