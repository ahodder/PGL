using PGL.Frontend;

namespace PGL.Ast;

public class AstStatement : IAstNode
{
}

public class AstStatementBlock : AstStatement
{
    public List<AstStatement> Statements { get; }
    public SymbolTable SymbolTable { get; set; }

    public AstStatementBlock(List<AstStatement> statements)
    {
        Statements = statements;
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

public class AstVariableAssignmentStatement : AstStatement
{
    public Token VariableIdentifierIdentifier { get; }
    public Token TypeIdentifierInformation { get; set; }
    public AstExpression Expression { get; }

    public AstVariableAssignmentStatement(Token variableIdentifier, Token typeIdentifier, AstExpression expression)
    {
        VariableIdentifierIdentifier = variableIdentifier;
        TypeIdentifierInformation = typeIdentifier;
        Expression = expression;
    }
}