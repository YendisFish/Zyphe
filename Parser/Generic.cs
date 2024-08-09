namespace Zyphe.Parser;

public record Generic(string name, List<Constraint> constraints);
public record GenericUsage(TypeInfo type);