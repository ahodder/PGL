using PGL.Frontend;

namespace PGL.Ast;

/// <summary>
/// Represents the symbol that is used to find types.
/// </summary>
public class PglType
{
    public static readonly PglType PrimitiveI8 = new PglType("i8");
    public static readonly PglType PrimitiveI16 = new PglType("i16");
    public static readonly PglType PrimitiveI32 = new PglType("i32");
    public static readonly PglType PrimitiveI64 = new PglType("i64");
    
    public static readonly PglType PrimitiveU8 = new PglType("u8");
    public static readonly PglType PrimitiveU16 = new PglType("u16");
    public static readonly PglType PrimitiveU32 = new PglType("u32");
    public static readonly PglType PrimitiveU64 = new PglType("u64");
    
    public static readonly PglType PrimitiveF32 = new PglType("f32");
    public static readonly PglType PrimitiveF64 = new PglType("f64");
    
    public static readonly PglType PrimitiveBool = new PglType("bool");
    
    public string Symbol => Information.Literal;
    public Token Information { get; }

    public PglType(string identifier)
    {
        Information = new Token(null, 0, 0, ETokenType.Identifier, identifier);
    }
}