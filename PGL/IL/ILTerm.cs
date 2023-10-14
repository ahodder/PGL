using System.Numerics;
using PGL.Ast;

namespace PGL.IL;

public class ILTerm
{
    public EILRegister DestinationRegister { get; }

    public ILTerm(EILRegister register)
    {
        DestinationRegister = register;
    }
}

public class ILIntegerLiteralTerm : ILTerm
{
    public PglType Type { get; }
    public BigInteger Literal { get; }

    public ILIntegerLiteralTerm(EILRegister destinationRegister, PglType type, string integerString) : base(destinationRegister)
    {
        Type = type;
        Literal = BigInteger.Parse(integerString);
    }
}

public class ILFloatLiteralTerm : ILTerm
{
    public PglType Type { get; }
    public double Literal { get; }

    public ILFloatLiteralTerm(EILRegister destinationRegister, PglType type, string floatString) : base(destinationRegister)
    {
        Type = type;
        Literal = double.Parse(floatString);
    }
}

public class ILStringLiteralTerm : ILTerm
{
    public PglType Type { get; }
    public string Literal { get; }

    public ILStringLiteralTerm(EILRegister destinationRegister, PglType type, string literal) : base(destinationRegister)
    {
        Type = type;
        Literal = literal;
    }
}

public class ILVariableTerm : ILTerm
{
    public PglType Type { get; }
    public string VariableName { get; }

    public ILVariableTerm(EILRegister destinationRegister, PglType type, string variableName) : base(destinationRegister)
    {
        Type = type;
        VariableName = variableName;
    }
}