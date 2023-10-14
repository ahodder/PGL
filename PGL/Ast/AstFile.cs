using PGL.Frontend;

namespace PGL.Ast;

public class AstFile
{
    public Token Source { get; }
    public List<AstFunction> Functions { get; }

    public AstFile(Token source, List<AstFunction> functions)
    {
        Source = source;
        Functions = functions;
    }
}