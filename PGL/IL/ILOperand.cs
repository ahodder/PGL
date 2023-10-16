namespace PGL.IL;

public class ILOperand
{
}

public class ILImmediateIntegerValueOperand : ILOperand
{
    public string Literal { get; }
    public int BitSize { get; }

    public ILImmediateIntegerValueOperand(EILRegister destinationRegister, int bitSize, string integerString)
    {
        BitSize = bitSize;
        Literal = integerString;
    }

    public override string ToString() => Literal;
}

public class ILImmediateFloatValueOperand : ILOperand
{
    public string Literal { get; }
    public int BitSize { get; }

    public ILImmediateFloatValueOperand(EILRegister destinationRegister, int bitSize, string floatString)
    {
        BitSize = bitSize;
        Literal = floatString;
    }
    
    public override string ToString() => Literal;
}

public class ILRegisterOperand : ILOperand
{
    public EILRegister Register { get; }

    public ILRegisterOperand(EILRegister register)
    {
        Register = register;
    }
    
    public override string ToString() => Register.ToString();
}

public class ILRelativeAddressOperand : ILOperand
{
    public EILRegister Register { get; }
    public int Offset { get; }
    
    public ILRelativeAddressOperand(EILRegister register, int offset)
    {
        Register = register;
        Offset = offset;
    }

    public override string ToString() => $"[{Register}, {Offset}]";
}