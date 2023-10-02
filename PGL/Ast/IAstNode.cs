using PGL.Frontend;

namespace PGL.Ast;

public interface IAstNode
{
}

public class AstFile : IAstNode
{
    public Token Source { get; }
    public List<AstFunction> Functions { get; }

    public AstFile(Token source, List<AstFunction> functions)
    {
        Source = source;
        Functions = functions;
    }
}

public class AstFunction : IAstNode
{
    public Token FunctionIdentifier { get; }
    public List<AstVariableTypeDeclaration> FunctionArguments { get; }
    public List<AstVariableTypeDeclaration> ReturnArguments { get; }
    public AstStatementBlock Statements { get; }


    public AstFunction(Token functionIdentifier, List<AstVariableTypeDeclaration> functionArguments, List<AstVariableTypeDeclaration> functionReturns, AstStatementBlock statements)
    {
        FunctionIdentifier = functionIdentifier;
        FunctionArguments = functionArguments;
        ReturnArguments = functionReturns;
        Statements = statements;
    }
}

public class AstStatementBlock : IAstNode
{
    public List<AstStatement> Statements { get; }

    public AstStatementBlock(List<AstStatement> statements)
    {
        Statements = statements;
    }
}

public class AstStatement : IAstNode
{
}

public class AstVariableAssignmentStatement : AstStatement
{
    public Token VariableIdentifier { get; }
    public AstTypeIdentifier TypeIdentifier { get; }
    public AstExpression Expression { get; }

    public AstVariableAssignmentStatement(Token variable, AstTypeIdentifier type, AstExpression expression)
    {
        VariableIdentifier = variable;
        TypeIdentifier = type;
        Expression = expression;
    }
}

public class AstReturnStatement : AstStatement
{
    public AstExpression Expression { get; }

    public AstReturnStatement(AstExpression expression)
    {
        Expression = expression;
    }
}

public class AstExpressionStatement : AstStatement
{
    public AstExpression Expression { get; }

    public AstExpressionStatement(AstExpression expression)
    {
        Expression = expression;
    }
}

public class AstExpression : IAstNode
{
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

public class AstTermExpression : AstExpression
{
}

public class AstIntegerLiteralExpression : AstTermExpression
{
    public Token IntegerLiteral { get; }

    public AstIntegerLiteralExpression(Token integerLiteral)
    {
        IntegerLiteral = integerLiteral;
    }
}

public class AstFloatLiteralExpression : AstTermExpression
{
    public Token FloatLiteral { get; }

    public AstFloatLiteralExpression(Token floatLiteral)
    {
        FloatLiteral = floatLiteral;
    }
}

public class AstStringLiteralExpression : AstTermExpression
{
    public Token StringLiteral { get; }

    public AstStringLiteralExpression(Token stringLiteral)
    {
        StringLiteral = stringLiteral;
    }
}

public class AstVariableDereferenceTermExpression : AstTermExpression
{
    public Token VariableIdentifier { get; }

    public AstVariableDereferenceTermExpression(Token variableIdentifier)
    {
        VariableIdentifier = variableIdentifier;
    }
}

public class AstFunctionInvocationExpression : AstTermExpression
{
    public Token FunctionIdentifier { get; }
    public List<AstExpression> FunctionParameters { get; }

    public AstFunctionInvocationExpression(Token functionIdentifier, List<AstExpression> functionParameters)
    {
        FunctionIdentifier = functionIdentifier;
        FunctionParameters = functionParameters;
    }
}

public class AstUnaryNegativeTerm : AstTermExpression
{
    public AstExpression Expression { get; }

    public AstUnaryNegativeTerm(AstExpression expression)
    {
        Expression = expression;
    }
}

public class AstVariableTypeDeclaration : IAstNode
{
    public Token? VariableIdentifier { get; }
    public AstTypeIdentifier TypeIdentifier { get; }

    public AstVariableTypeDeclaration(Token? variableIdentifier, AstTypeIdentifier typeIdentifier)
    {
        VariableIdentifier = variableIdentifier;
        TypeIdentifier = typeIdentifier;
    }
}

public class AstTypeIdentifier : IAstNode
{
    public Token Identifier { get; }

    public AstTypeIdentifier(Token identifier)
    {
        Identifier = identifier;
    }
}