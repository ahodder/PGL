using PGL.Frontend;

namespace PGL.Ast;

public class AstFunction : IAstNode
{
    public Token FunctionIdentifier { get; }
    public List<AstVariableTypeDeclaration> FunctionArguments { get; }
    public List<AstVariableTypeDeclaration> ReturnArguments { get; }
    public AstStatementBlock Statements { get; }
    public SymbolTable SymbolTable { get; set; }


    public AstFunction(Token functionIdentifier, List<AstVariableTypeDeclaration> functionArguments, List<AstVariableTypeDeclaration> functionReturns, AstStatementBlock statements)
    {
        FunctionIdentifier = functionIdentifier;
        FunctionArguments = functionArguments;
        ReturnArguments = functionReturns;
        Statements = statements;
    }
}