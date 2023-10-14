using PGL.Frontend;

namespace PGL.Ast;

public class AstStatement : IAstNode
{
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

public class AstVariableAssignmentStatement : AstStatement
{
    public Token VariableIdentifier { get; }
    public AstTypeIdentifier TypeIdentifier { get; set; }
    public AstExpression Expression { get; }

    public AstVariableAssignmentStatement(Token variable, AstTypeIdentifier type, AstExpression expression)
    {
        VariableIdentifier = variable;
        TypeIdentifier = type;
        Expression = expression;
    }
}