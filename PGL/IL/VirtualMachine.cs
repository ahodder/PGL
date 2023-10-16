using PGL.Core;

namespace PGL.IL;

public class VirtualMachine
{
    private Memory<byte>[] _registers;
    private Memory<byte> _mainMemory;

    public VirtualMachine(Configuration configuration, uint memorySize)
    {
        var registers = Enum.GetValues<EILRegister>();
        _registers = new Memory<byte>[registers.Length];
        _mainMemory = new Memory<byte>(new byte[memorySize]);
        for (var i = 0; i < registers.Length; i++)
            _registers[i] = new Memory<byte>(new byte[configuration.TargetPlatformInstructionSizeBytes]);
    }

    public void ExecuteInstruction(ILInstruction instruction)
    {
        switch (instruction.Instruction)
        {
            default:
                throw new Exception($"Virtual Machine failed: unable to handle unexpected instruction: {instruction.Instruction}");
        }
    }
}