using PGL.Frontend;

namespace PGL.Ast;

public abstract class AstTerm : AstExpression
{
}

public class AstIntegerLiteralTerm : AstTerm
{
    public Token IntegerLiteral { get; }

    public AstIntegerLiteralTerm(Token integerLiteral)
    {
        IntegerLiteral = integerLiteral;
    }

    public override string ToString() => IntegerLiteral.Literal;
}


public class AstFunctionInvocationTerm : AstTerm
{
    public Token FunctionIdentifier { get; }
    public List<AstExpression> FunctionParameters { get; }

    public AstFunctionInvocationTerm(Token functionIdentifier, List<AstExpression> functionParameters)
    {
        FunctionIdentifier = functionIdentifier;
        FunctionParameters = functionParameters;
    }
    
    public override string ToString() => FunctionIdentifier.Literal;
}

public class AstFloatLiteralTerm : AstTerm
{
    public Token FloatLiteral { get; }

    public AstFloatLiteralTerm(Token floatLiteral)
    {
        FloatLiteral = floatLiteral;
    }
    
    public override string ToString() => FloatLiteral.Literal;
}

public class AstStringLiteralTerm : AstTerm
{
    public Token StringLiteral { get; }

    public AstStringLiteralTerm(Token stringLiteral)
    {
        StringLiteral = stringLiteral;
    }
    
    public override string ToString() => StringLiteral.Literal;
}

public class AstVariableDereferenceTerm : AstTerm
{
    public Token VariableIdentifier { get; }

    public AstVariableDereferenceTerm(Token variableIdentifier)
    {
        VariableIdentifier = variableIdentifier;
    }

    public override string ToString() => VariableIdentifier.Literal;
}

public class AstUnaryNegativeTerm : AstTerm
{
    public AstExpression Expression { get; }
    
    public AstUnaryNegativeTerm(AstExpression expression)
    {
        Expression = expression;
    }

    public override string ToString() => $"-{Expression}";
}
