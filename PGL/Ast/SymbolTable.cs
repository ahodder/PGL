namespace PGL.Ast;

public class SymbolInformation
{
    public AstTypeInformation TypeInformation { get; }
    public int StackOffset { get; }

    public SymbolInformation(AstTypeInformation typeInformation, int stackOffset)
    {
        TypeInformation = typeInformation;
        StackOffset = stackOffset;
    }
}

/// <summary>
/// Represents how symbols are stored during the runtime of an application. This will manage memory layout of declared symbols. In
/// effect, this table is the collection of all stack frames and code blocks for a single thread of execution. Note: a code block is
/// not quite a stack frame, but acts like one in many respects.
/// </summary>
public class SymbolTable
{
    public int StackSize => (_parent?.StackSize ?? 0) + _stackSymbols.Select(x => x.TypeInformation.ByteSize).Sum();
    
    private List<SymbolInformation> _stackSymbols = new List<SymbolInformation>();
    private readonly Dictionary<string, int> _symbolMapping = new Dictionary<string, int>();
    private readonly SymbolTable _parent;

    public SymbolTable()
    {
    }

    private SymbolTable(SymbolTable parent)
    {
        _parent = parent;
    }

    public void RegisterSymbolWithType(string symbol, AstTypeInformation type)
    {
        if (_symbolMapping.ContainsKey(symbol))
            throw new Exception($"Cannot register symbol, symbol {symbol} already exists");

        _symbolMapping[symbol] = _stackSymbols.Count;
        _stackSymbols.Add(new SymbolInformation(type, StackSize));
    }

    public SymbolInformation FindSymbol(string symbol)
    {
        if (!TryFindSymbol(symbol, out var ret))
            throw new Exception($"Failed to find symbol: {symbol}");

        return ret;
    }

    public bool TryFindSymbol(string symbol, out SymbolInformation outType)
    {
        if (!_symbolMapping.TryGetValue(symbol, out var index))
        {
            if (_parent != null)
                return _parent.TryFindSymbol(symbol, out outType);
        }

        outType = _stackSymbols[index];
        return true;
    }

    public SymbolTable CreateSubTable() => new SymbolTable(this);
}