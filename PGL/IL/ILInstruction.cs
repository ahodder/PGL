using System.Text;

namespace PGL.IL;

public class ILInstruction
{
    public EILInstruction Instruction { get; }
    public EILRegister DestinationRegister { get; }
    public ILOperand LeftOperand { get; }
    public ILOperand RightOperand { get; }
    public int ByteSize { get; }
    public string Comment { get; set; }

    public ILInstruction(EILInstruction instruction, EILRegister register, ILOperand leftOperand, ILOperand rightOperand, int byteSize = 0, string comment = null)
    {
        Instruction = instruction;
        DestinationRegister = register;
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        ByteSize = byteSize;
        Comment = comment;
    }

    public override string ToString()
    {
        if (Instruction == EILInstruction.Nop)
            return Instruction.ToString();
        if (Instruction == EILInstruction.Function)
            return $"\n{Comment}";

        var sb = new StringBuilder();

        sb.Append("\t")
            .Append($"{Instruction, -8}");

        if (LeftOperand != null)
        {
            if (DestinationRegister != EILRegister.Nop)
                sb.Append($"{DestinationRegister }, ");
            
            sb.Append(LeftOperand);
            if (RightOperand != null)
                sb.Append($", {RightOperand}");
        }

        return Comment != null ? $"{sb,-32} ; {Comment}" : sb.ToString();
    }
}