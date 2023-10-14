using PGL.Ast;

namespace PGL.IL;

public class ILInstruction
{
}

public class ILVariableDereferenceInstruction : ILInstruction
{
    public EILRegister TargetRegister { get; }
    public string VariableSymbol { get; }
    public PglType VariableType { get; }

    public ILVariableDereferenceInstruction(EILRegister targetRegister, string variableSymbol, PglType variableType)
    {
        TargetRegister = targetRegister;
        VariableSymbol = variableSymbol;
        VariableType = variableType;
    }
}

public class ILBinaryInstruction : ILInstruction
{
    public EILRegister TargetRegister { get; }
    public EBinaryOperator Operator { get; set; }
    public ILTerm LeftOperand { get; }
    public ILTerm RightOperand { get; }

    public ILBinaryInstruction(EILRegister targetRegister, ILTerm leftOperand, EBinaryOperator @operator, ILTerm rightOperand)
    {
        TargetRegister = targetRegister;
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }
}

public class ILReturnInstruction : ILInstruction
{
}