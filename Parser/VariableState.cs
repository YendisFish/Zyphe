using System.Runtime.InteropServices.JavaScript;

namespace Zyphe.Parser;

public class VariableState
{
    public List<Declaration.VariableDeclaration> Variables { get; set; } = new();
    public List<Declaration.VariableDeclaration> Globals { get; set; } = new();
    public List<Declaration.VariableDeclaration> Props { get; set; } = new();
    public List<Declaration.FunctionDeclaration> Funcs { get; set; } = new();
    public List<Declaration.FunctionDeclaration> GlobalFuncs { get; set; } = new();
}