namespace PGL.Ast;

public class AstStatementBlock : IAstNode
{
    public List<AstStatement> Statements { get; }

    public AstStatementBlock(List<AstStatement> statements)
    {
        Statements = statements;
    }
}