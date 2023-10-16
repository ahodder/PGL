using PGL.Core;
using PGL.IL;

namespace PGL.Ast;

public class AstProgram : IAstNode
{
    public List<AstFunction> Functions { get; }
    public TypeTable TypeTable { get; }
    public SymbolTable SymbolTable { get; }

    public AstProgram(Configuration configuration, List<AstFunction> functions)
    {
        Functions = functions;
        TypeTable = new TypeTable(configuration);
        SymbolTable = new SymbolTable();
    }
}