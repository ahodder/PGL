namespace PGL.Ast;

public abstract class AstExpression : IAstNode
{
    public PglType ResolvedType { get; set; }
}

public class AstBinaryExpression : AstExpression
{
    public AstExpression LeftExpression { get; }
    public AstBinaryOperator Operation { get; }
    public AstExpression RightExpression { get; }

    public AstBinaryExpression(AstExpression leftExpression, AstBinaryOperator operation, AstExpression rightExpression)
    {
        LeftExpression = leftExpression;
        Operation = operation;
        RightExpression = rightExpression;
    }
}