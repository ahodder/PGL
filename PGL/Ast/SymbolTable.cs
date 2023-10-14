using PGL.Core;

namespace PGL.Ast;

public class SymbolTable
{
    private readonly Dictionary<string, PglType> _symbolMapping = new Dictionary<string, PglType>();

    private readonly SymbolTable _parent;

    private SymbolTable()
    {
    }

    private SymbolTable(SymbolTable parent)
    {
        _parent = parent;
    }

    public void RegisterSymbolWithType(string symbol, PglType type)
    {
        if (_symbolMapping.ContainsKey(symbol))
            throw new Exception($"Cannot register symbol, type {type.Symbol} already exists");

        _symbolMapping[symbol] = type;
    }

    public PglType FindType(string symbol)
    {
        PglType ret;

        if (!_symbolMapping.TryGetValue(symbol, out ret) && !TryFindType(symbol, out ret))
            throw new Exception($"Failed to find symbol: {symbol}");

        return ret;
    }

    public bool TryFindType(string symbol, out PglType outType)
    {
        if (!_symbolMapping.TryGetValue(symbol, out outType))
        {
            if (_parent != null)
                return _parent.TryFindType(symbol, out outType);
        }

        return true;
    }

    public SymbolTable CreateSubSymbolTable() => new SymbolTable(this);

    public static SymbolTable CreateRootSymbolTable(Configuration configuration)
    {
        var ret = new SymbolTable();

        void RegisterPrimitive(PglType type)
        {
            ret._symbolMapping[type.Symbol] = type;
        }

        RegisterPrimitive(new PglType("int", true, configuration.TargetPlatformInstructionSizeBytes));
        RegisterPrimitive(PglType.PrimitiveI8);
        RegisterPrimitive(PglType.PrimitiveI32);
        RegisterPrimitive(PglType.PrimitiveI64);

        RegisterPrimitive(new PglType("uint", true, configuration.TargetPlatformInstructionSizeBytes));
        RegisterPrimitive(PglType.PrimitiveU8);
        RegisterPrimitive(PglType.PrimitiveU16);
        RegisterPrimitive(PglType.PrimitiveU32);
        RegisterPrimitive(PglType.PrimitiveU64);

        RegisterPrimitive(new PglType("float", true, configuration.TargetPlatformInstructionSizeBytes));
        RegisterPrimitive(PglType.PrimitiveF32);
        RegisterPrimitive(PglType.PrimitiveF64);

        RegisterPrimitive(PglType.PrimitiveBool);

        return ret;
    } 
}