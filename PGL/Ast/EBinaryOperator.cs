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
}