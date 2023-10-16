namespace PGL.IL;

public class InstructionUnit
{
    public List<ILInstruction> Instructions { get; private set; } = new List<ILInstruction>();

    public InstructionUnit()
    {
    }

    #region Arithmetic

    public InstructionUnit Addi(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int byteSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Addi, destinationRegister, leftOperand, rightOperand, byteSize, comment));
        return this;
    }
    
    public InstructionUnit Addu(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int byteSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Addu, destinationRegister, leftOperand, rightOperand, byteSize, comment));
        return this;
    }
    
    public InstructionUnit Addf(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Addf, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Subi(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Subi, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Subu(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Subu, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Subf(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Subf, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Muli(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Muli, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Mulu(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Mulu, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Mulf(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Mulf, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Divi(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Divi, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Divu(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Divu, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }
    
    public InstructionUnit Divf(EILRegister destinationRegister, ILOperand leftOperand, ILOperand rightOperand, int bitSize, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Divf, destinationRegister, leftOperand, rightOperand, bitSize, comment));
        return this;
    }

    #endregion Arithmetic


    #region DataMovement

    public InstructionUnit Mov(ILRegisterOperand destinationRegister, ILOperand dataSource, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Mov, EILRegister.Nop, destinationRegister, dataSource, comment: comment));
        return this;
    }
    
    public InstructionUnit Mov(ILRelativeAddressOperand destinationRegister, ILOperand dataSource, string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Mov, EILRegister.Nop, destinationRegister, dataSource, comment: comment));
        return this;
    }
    
    #endregion DataMovement

    public InstructionUnit Func(string functionName)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Function, EILRegister.Nop, null, null, comment: functionName));
        return this;
    }

    public InstructionUnit Return(string comment = null)
    {
        Instructions.Add(new ILInstruction(EILInstruction.Return, EILRegister.Nop, null, null, comment: comment));
        return this;
    }
}