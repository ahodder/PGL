namespace PGL.IL;

public enum EILRegister
{
    Nop,
    R1,
    R2,
    R3,
    R4,
    R5,
    R6,
    R7,
    R8,
    /// <summary>
    /// Registers that should not cross a function boundary. These are temporary registers used for ad hoc computation.
    /// </summary>
    RTmp1,
    /// <summary>
    /// Registers that should not cross a function boundary. These are temporary registers used for ad hoc computation.
    /// </summary>
    RTmp2,
    RIP,
    RSP,
}