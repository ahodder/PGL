namespace PGL.IL;

public enum EILInstruction
{
    Nop,
    
    Add,
    Sub,
    Mul,
    Div,
    
    Function,
    Return,
    
    /// <summary>
    /// The move instruction does not use the destination register as stored in the instruction. Instead it uses the
    /// left operand as the destination and the right operand as the data.
    /// </summary>
    Mov,
}