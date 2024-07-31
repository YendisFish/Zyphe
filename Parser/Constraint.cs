namespace Zyphe.Parser;

public abstract record Constraint
{
    public record TypeConstraint(string Name) : Constraint;
    public record IdentifierConstraint(VariableIdentifier Identifier) : Constraint;
}