namespace PGL.IL;

public enum EILInstruction
{
    Nop,
    
    /*
    Addi8,
    Addi16,
    Addi32,
    Addi64,
    
    Addu8,
    Addu16,
    Addu32,
    Addu64,
    
    Addf32,
    Addf64,
    */

    Addi,
    Addu,
    Addf,
    Subi,
    Subu,
    Subf,
    Muli,
    Mulu,
    Mulf,
    Divi,
    Divu,
    Divf,
    
    Function,
    Return,
    
    /// <summary>
    /// The move instruction does not use the destination register as stored in the instruction. Instead it uses the
    /// left operand as the destination and the right operand as the data.
    /// </summary>
    Mov,
}