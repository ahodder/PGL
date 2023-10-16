using PGL.Frontend;

namespace PGL.Ast;

[Flags]
public enum ETypeFlags
{
    None = 0,
    Numeric = 1,
    Integer = 1 << 1 | Numeric,
    Real = 1 << 2 | Numeric,
    Value = 1 << 3,
    Reference = 1 << 4,
    Signed = 1 << 5,
    
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