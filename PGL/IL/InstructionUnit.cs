namespace PGL.IL;

public class InstructionUnit
{
    public List<ILInstruction> Instructions { get; private set; } = new List<ILInstruction>();

    public InstructionUnit()
    {
    }

    #region Arithmetic

    public InstructionUnit Add(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Add, destinationRegister, leftOperand, rightOperand, comment));
        return this;
    }
    
    public InstructionUnit Sub(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Sub, destinationRegister, leftOperand, rightOperand, comment));
        return this;
    }
    
    public InstructionUnit Mul(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Mul, destinationRegister, leftOperand, rightOperand, comment));
        return this;
    }
    
    public InstructionUnit Div(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Div, destinationRegister, leftOperand, rightOperand, comment));
        return this;
    }

    #endregion Arithmetic


    #region DataMovement

    public InstructionUnit Mov(ILRegisterOperand destinationRegister, ILOperand dataSource, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Mov, EILRegister.Nop, destinationRegister, dataSource, comment));
        return this;
    }
    
    public InstructionUnit Mov(ILRelativeAddressOperand destinationRegister, ILOperand dataSource, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Mov, EILRegister.Nop, destinationRegister, dataSource, comment));
        return this;
    }
    
    #endregion DataMovement

    public InstructionUnit Func(string functionName)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Function, EILRegister.Nop, null, null, functionName));
        return this;
    }

    public InstructionUnit Return(string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Return, EILRegister.Nop, null, null, comment));
        return this;
    }
}