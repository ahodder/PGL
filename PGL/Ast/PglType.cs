using PGL.Frontend;

namespace PGL.Ast;

public class PglType
{
    public static readonly PglType PrimitiveI8 = new PglType("i8", true, 1);
    public static readonly PglType PrimitiveI16 = new PglType("i16", true, 2);
    public static readonly PglType PrimitiveI32 = new PglType("i32", true, 4);
    public static readonly PglType PrimitiveI64 = new PglType("i64", true, 8);
    
    public static readonly PglType PrimitiveU8 = new PglType("u8", true, 1);
    public static readonly PglType PrimitiveU16 = new PglType("u16", true, 2);
    public static readonly PglType PrimitiveU32 = new PglType("u32", true, 4);
    public static readonly PglType PrimitiveU64 = new PglType("u64", true, 8);
    
    public static readonly PglType PrimitiveF32 = new PglType("f32", true, 4);
    public static readonly PglType PrimitiveF64 = new PglType("f64", true, 8);
    
    public static readonly PglType PrimitiveBool = new PglType("bool", true, 1);
    
    public string Symbol => Identifier.Identifier.Literal;
    public AstTypeIdentifier Identifier { get; }
    public bool IsNumeric { get; }
    public bool IsPrimitive { get; }
    public uint ByteSize { get; }

    public PglType(AstTypeIdentifier identifier, bool isNumeric, uint byteSize)
    {
        Identifier = identifier;
        IsNumeric = isNumeric;
        IsPrimitive = false;
        ByteSize = byteSize;
    }

    public PglType(string identifier, bool isNumeric, uint byteSize)
    {
        Identifier = new AstTypeIdentifier(new Token(null, 0, 0, ETokenType.Identifier, identifier));
        IsNumeric = isNumeric;
        IsPrimitive = true;
        ByteSize = byteSize;
    }
}