using PGL.Frontend;

namespace PGL.Ast;

public class AstVariableTypeDeclaration : IAstNode
{
    public Token VariableIdentifier { get; set; }
    public Token TypeIdentifier { get; }
    public AstTypeInformation TypeInformation { get; set; }
    
    public AstVariableTypeDeclaration(Token variableIdentifier, Token typeIdentifier)
    {
        VariableIdentifier = variableIdentifier;
        TypeIdentifier = typeIdentifier;
    }
}