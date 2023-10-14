using PGL.Ast;

namespace PGL.IL;

public class ILFunctionArgument
{
    public AstVariableTypeDeclaration Source { get; }
    public string Name { get; }
    public PglType Type { get; }
}

public class ILFunctionReturn
{
    public AstVariableTypeDeclaration Source { get; }
    public string Name { get; }
    public PglType Type { get; }
}

public class ILFunction
{
    public AstFunction Source { get; }
    
    public string Name { get; }
    
    public List<ILFunctionArgument> Arguments { get; }
    public List<ILFunctionReturn> Returns { get; }
    public List<ILInstruction> Statements { get; }
}