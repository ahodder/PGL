using PGL.Frontend;

namespace PGL.Ast;

[Flags]
public enum ETypeFlags
{
    None = 0,
    Numeric = 1,
    Value = 1 << 1,
    Reference = 1 << 2,
    
    Primitive = Numeric | Value,
}

public class AstTypeInformation : IAstNode
{
    public Token Identifier { get; }
    public ETypeFlags TypeFlags { get; set; }
    /// <summary>
    /// The amount of memory needed to represent the type.
    /// </summary>
    public int ByteSize { get; set; }

    public AstTypeInformation(Token identifier)
    {
        Identifier = identifier;
    }
}