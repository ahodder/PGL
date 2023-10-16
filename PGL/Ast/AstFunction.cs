using System.Text;
using PGL.Frontend;

namespace PGL.Ast;

public class AstFunction : IAstNode
{
    public Token FunctionIdentifier { get; }
    public List<AstVariableTypeDeclaration> FunctionArguments { get; }
    public List<AstVariableTypeDeclaration> ReturnArguments { get; }
    public AstStatementBlock Statements { get; }
    public SymbolTable ArgsAndRetsSymbolTable { get; set; }


    public AstFunction(Token functionIdentifier, List<AstVariableTypeDeclaration> functionArguments, List<AstVariableTypeDeclaration> functionReturns, AstStatementBlock statements)
    {
        FunctionIdentifier = functionIdentifier;
        FunctionArguments = functionArguments;
        ReturnArguments = functionReturns;
        Statements = statements;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(FunctionIdentifier.Literal).Append("");
        if (FunctionArguments?.Count > 0)
        {
            sb.Append("(");
            sb.Append(string.Join(", ", FunctionArguments.Select(x => $"{x.VariableIdentifier.Literal}: {x.TypeIdentifier}")));
            sb.Append(")");
        }

        if (ReturnArguments?.Count > 0)
        {
            sb.Append(" = ");
            sb.Append(string.Join(", ", ReturnArguments.Select(x => $"{x.VariableIdentifier?.Literal ?? "inf"}: {x.TypeIdentifier.Literal}")));
        }

        return sb.ToString();
    }
}