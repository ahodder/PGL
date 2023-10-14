using PGL.Frontend;

namespace PGL.Ast;

public class AstTypeIdentifier : IAstNode
{
    public Token Identifier { get; }
    
    public PglType ResolvedType { get; set; }

    public AstTypeIdentifier(Token identifier)
    {
        Identifier = identifier;
    }
}