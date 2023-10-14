using PGL.Frontend;

namespace PGL.Ast;

public class AstBinaryOperator : IAstNode
{
    public Token Source { get; }
    public EBinaryOperator Operation { get; }

    public AstBinaryOperator(Token source, EBinaryOperator operation)
    {
        Source = source;
        Operation = operation;
    }
}