using PGL.Ast;
using PGL.Core;
using PGL.IL;

namespace PGL.Frontend;

public class ILCodeGenerator
{
    private Configuration _configuration;
    private AstProgram _program;

    public ILCodeGenerator(Configuration configuration, AstProgram program)
    {
        _configuration = configuration;
        _program = program;
    }

    public void GenerateILCode()
    {
        foreach (var function in _program.Functions)
        {
            AnalyzeFunction(function);
        }

        ;
    }

    public void AnalyzeFunction(AstFunction function)
    {
        var instructions = new List<ILInstruction>();

        foreach (var statement in function.Statements.Statements)
        {
            switch (statement)
            {
                case AstExpressionStatement expressionStatement:
                    AnalyzeExpression(EILRegister.R1, expressionStatement.Expression, instructions);
                    break;
                
                case AstReturnStatement returnStatement:
                    AnalyzeExpression(EILRegister.R1, returnStatement.Expression, instructions);
                    instructions.Add(new ILReturnInstruction());
                    break;
                
                default:
                    throw new Exception($"Cannot analyze function: unexpected statement: {statement.GetType().Name}"); 
            }
        }
    }

    public void AnalyzeExpression(EILRegister destinationRegister, AstExpression expression, List<ILInstruction> instructions)
    {
        switch (expression)
        {
            case AstBinaryExpression binaryExpression:
                AnalyzeBinaryExpression(destinationRegister, binaryExpression, instructions);
                break;
        }
    }

    public void AnalyzeBinaryExpression(EILRegister destinationRegister, AstBinaryExpression expression, List<ILInstruction> instructions)
    {
        if (expression.LeftExpression is AstBinaryExpression left)
            AnalyzeBinaryExpression(EILRegister.RTmp1, left, instructions);
        
        if (expression.LeftExpression is AstBinaryExpression right)
            AnalyzeBinaryExpression(EILRegister.RTmp2, right, instructions);

        var leftTerm = BuildAndAnalyzeTerm(EILRegister.RTmp1, (AstTerm)expression.LeftExpression);
        var rightTerm = BuildAndAnalyzeTerm(EILRegister.RTmp2, (AstTerm)expression.RightExpression);
        var binaryInstruction = new ILBinaryInstruction(destinationRegister, leftTerm, expression.Operation.Operation, rightTerm);

        instructions.Add(binaryInstruction);
    }

    public ILTerm BuildAndAnalyzeTerm(EILRegister destinationRegister, AstTerm term)
    {
        switch (term)
        {
            // case AstIntegerLiteralTerm intTerm:
            //     return new ILIntegerLiteralTerm(destinationRegister, PglType.Ptimi, intTerm.IntegerLiteral.Literal);
            //
            // case AstFloatLiteralTerm floatTerm:
            //     return new ILFloatLiteralTerm(destinationRegister, _primitiveFloat, floatTerm.FloatLiteral.Literal);
            //
            // // case AstStringLiteralTerm stringTerm:
            // //     return new ILStringLiteralTerm(_primitiveSt, stringTerm.StringLiteral.Literal);
            //
            // case AstVariableDereferenceTerm variableTerm:
            //     return new ILVariableTerm(destinationRegister, _primitiveInt, variableTerm.VariableIdentifier.Literal);
            
            default:
                throw new Exception($"Cannot build term: unrecognized term type: {term}");
        }
    }
}