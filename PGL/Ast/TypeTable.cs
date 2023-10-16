using PGL.Core;

namespace PGL.Ast;

/// <summary>
/// Stores the type information for the program. This is how the types are used, but instead stores how they should be used.
/// </summary>
public class TypeTable
{
    private readonly Dictionary<string, AstTypeInformation> _symbolMapping = new Dictionary<string, AstTypeInformation>();

    private readonly Configuration _configuration;

    public TypeTable(Configuration configuration)
    {
        _configuration = configuration;
        
        void RegisterPrimitive(PglType type, int byteSize)
        {
            var typeInfo = new AstTypeInformation(type.Information);
            typeInfo.ByteSize = byteSize;
            typeInfo.TypeFlags = ETypeFlags.Primitive;
            _symbolMapping[type.Symbol] = typeInfo;
        }

        RegisterPrimitive(new PglType("int"), configuration.TargetPlatformInstructionSizeBytes);
        RegisterPrimitive(PglType.PrimitiveI8, 1);
        RegisterPrimitive(PglType.PrimitiveI16, 2);
        RegisterPrimitive(PglType.PrimitiveI32, 4);
        RegisterPrimitive(PglType.PrimitiveI64, 8);

        RegisterPrimitive(new PglType("uint"), configuration.TargetPlatformInstructionSizeBytes);
        RegisterPrimitive(PglType.PrimitiveU8, 1);
        RegisterPrimitive(PglType.PrimitiveU16, 2);
        RegisterPrimitive(PglType.PrimitiveU32, 4);
        RegisterPrimitive(PglType.PrimitiveU64, 8);

        RegisterPrimitive(new PglType("float"), configuration.TargetPlatformInstructionSizeBytes);
        RegisterPrimitive(PglType.PrimitiveF32, 4);
        RegisterPrimitive(PglType.PrimitiveF64, 8);

        RegisterPrimitive(PglType.PrimitiveBool, 1);
    }


    public void RegisterType(string symbol, AstTypeInformation type)
    {
        if (_symbolMapping.ContainsKey(symbol))
            throw new Exception($"Cannot register type: {type.Identifier.Literal} already exists");

        _symbolMapping[symbol] = type;
    }

    public AstTypeInformation FindType(string symbol)
    {
        AstTypeInformation ret;

        if (!_symbolMapping.TryGetValue(symbol, out ret) && !TryFindType(symbol, out ret))
            throw new Exception($"Failed to find symbol: {symbol}");

        return ret;
    }

    public bool TryFindType(string symbol, out AstTypeInformation outType)
    {
        return !_symbolMapping.TryGetValue(symbol, out outType);
    }
}