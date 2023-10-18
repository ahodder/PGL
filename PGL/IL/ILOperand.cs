namespace PGL.IL;

public class ILOperand
{
}

public class ILImmediateIntegerValueOperand : ILOperand
{
    public bool Signed { get; }
    public int ByteSize { get; }
    public string Literal { get; }

    public ILImmediateIntegerValueOperand(bool signed, int byteSize, string integerString)
    {
        Signed = signed;
        ByteSize = byteSize;
        Literal = integerString;
    }

    public override string ToString() => Literal;
}

public class ILImmediateFloatValueOperand : ILOperand
{
    public string Literal { get; }
    public int ByteSize { get; }

    public ILImmediateFloatValueOperand(int byteSize, string floatString)
    {
        ByteSize = byteSize;
        Literal = floatString;
    }
    
    public override string ToString() => Literal;
}

public class ILRegisterOperand : ILOperand
{
    public EILRegister Register { get; }
    public int ByteSize { get; }

    public ILRegisterOperand(EILRegister register, int byteSize)
    {
        Register = register;
        ByteSize = byteSize;
    }
    
    public override string ToString() => Register.ToString();
}

public class ILRelativeAddressOperand : ILRegisterOperand
{
    public int Offset { get; }
    
    public ILRelativeAddressOperand(EILRegister register, int offset, int byteSize) : base(register, byteSize)
    {
        Offset = offset;
    }

    public override string ToString() => $"[{Register}, {Offset}]";
}