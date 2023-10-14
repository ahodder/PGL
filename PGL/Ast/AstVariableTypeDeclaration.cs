using PGL.Frontend;

namespace PGL.Ast;

public class AstVariableTypeDeclaration : IAstNode
{
    public Token? VariableIdentifier { get; }
    public AstTypeIdentifier TypeIdentifier { get; }
    public PglType ResolvedType { get; set; }

    public AstVariableTypeDeclaration(Token? variableIdentifier, AstTypeIdentifier typeIdentifier)
    {
        VariableIdentifier = variableIdentifier;
        TypeIdentifier = typeIdentifier;
    }
}