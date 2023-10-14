using PGL.IL;

namespace PGL.Ast;

public class AstProgram : IAstNode
{
    public List<AstFunction> Functions { get; }
    public SymbolTable SymbolTable { get; set; }

    public AstProgram(List<AstFunction> functions)
    {
        Functions = functions;
    }
}