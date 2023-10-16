namespace PGL.Ast;

public enum EBinaryOperator
{
    Subtraction,
    Addition,
    Division,
    Multiplication,
}

public static class BinaryOperatorExtensions
{
    public static bool IsHigherPrecedenceThan(this EBinaryOperator first, EBinaryOperator second)
    {
        return first >= second;
    }

    public static string AsString(this EBinaryOperator op)
    {
        switch (op)
        {
            case EBinaryOperator.Subtraction: return "-";
            case EBinaryOperator.Addition: return "+";
            case EBinaryOperator.Division: return "/";
            case EBinaryOperator.Multiplication: return "*";
            default: throw new Exception("Cannot stringify operator");
        }
    }
}