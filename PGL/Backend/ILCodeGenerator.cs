using System.Text;
using PGL.Ast;
using PGL.Core;
using PGL.IL;

namespace PGL.Backend;

public class ILCodeGenerator
{
    public List<ILInstruction> Instructions => _instructions.Instructions;
    
    private Configuration _configuration;
    private AstProgram _program;
    public InstructionUnit _instructions;

    public ILCodeGenerator(Configuration configuration, AstProgram program)
    {
        _configuration = configuration;
        _program = program;
        _instructions = new InstructionUnit();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var instruction in Instructions)
            sb.AppendLine(instruction.ToString());

        return sb.ToString();
    }

    public void GenerateILCode()
    {
        foreach (var function in _program.Functions)
        {
            AnalyzeFunction(function);
        }
    }

    public void AnalyzeFunction(AstFunction function)
    {
        _instructions.Func(function.ToString());
        AnalyzeStatement(function.Statements.SymbolTable, function, function.Statements);
    }

    public void AnalyzeStatement(SymbolTable symbolTable, AstFunction function,  AstStatement statement)
    {
        switch (statement)
        {
            case AstExpressionStatement expressionStatement:
                AnalyzeExpression(symbolTable, EILRegister.R1, expressionStatement.Expression);
                break;

            case AstReturnStatement returnStatement:
            {
                var dest = EILRegister.R1;
                AnalyzeExpression(symbolTable, dest, returnStatement.Expression);
                var retIdent = function.ReturnArguments[0].VariableIdentifier.Literal;
                var symbolInfo = function.ArgsAndRetsSymbolTable.FindSymbol(retIdent);
                var comment = $"{retIdent} = {dest}";
                var byteSize = symbolInfo.TypeInformation.ByteSize;
                _instructions.Mov(new ILRelativeAddressOperand(EILRegister.RSP, symbolInfo.StackOffset, byteSize), new ILRegisterOperand(dest, byteSize), comment);
                _instructions.Return();
                break;
            }

            case AstVariableDeclarationStatement declarationStatement:
            {
                var dest = EILRegister.R1;
                AnalyzeExpression(symbolTable, dest, declarationStatement.Expression);
                var symbolInfo = symbolTable.FindSymbol(declarationStatement.VariableIdentifierIdentifier.Literal);
                var byteSize = symbolInfo.TypeInformation.ByteSize;
                _instructions.Mov(new ILRelativeAddressOperand(EILRegister.RSP, symbolInfo.StackOffset, byteSize), new ILRegisterOperand(dest, byteSize), $"{declarationStatement.VariableIdentifierIdentifier.Literal} = {dest}");
                break;
            }

            case AstVariableAssignmentStatement assignmentStatement:
            {
                var dest = EILRegister.R1;
                AnalyzeExpression(symbolTable, dest, assignmentStatement.Expression);
                var symbolInfo = symbolTable.FindSymbol(assignmentStatement.VariableIdentifierIdentifier.Literal);
                var byteSize = symbolInfo.TypeInformation.ByteSize;
                _instructions.Mov(new ILRelativeAddressOperand(EILRegister.RSP, symbolInfo.StackOffset, byteSize), new ILRegisterOperand(dest, byteSize), $"{assignmentStatement.VariableIdentifierIdentifier.Literal} = {dest}");
                break;
            }

            case AstStatementBlock statementBlock:
                foreach (var nested in statementBlock.Statements)
                {
                    AnalyzeStatement(statementBlock.SymbolTable, function, nested);
                }
                break;
                
            default:
                throw new Exception($"Cannot analyze function: unexpected statement: {statement.GetType().Name}"); 
        }
    }

    public void AnalyzeExpression(SymbolTable symbolTable, EILRegister destinationRegister, AstExpression expression)
    {
        switch (expression)
        {
            case AstBinaryExpression binaryExpression:
                AnalyzeBinaryExpression(symbolTable, destinationRegister, binaryExpression);
                break;
        }
    }

    public void AnalyzeBinaryExpression(SymbolTable symbolTable, EILRegister destinationRegister, AstBinaryExpression expression)
    {
        ILOperand leftOperand;
        ILOperand rightOperand;
        StringBuilder comment = new StringBuilder();
        
        if (expression.LeftExpression.TypeInformation.ByteSize != expression.RightExpression.TypeInformation.ByteSize)
            throw new Exception("Cannot emit binary operation instruction: type bit sizes are not congruent");

        var byteSize = expression.LeftExpression.TypeInformation.ByteSize;

        if (expression.LeftExpression is AstTerm leftTerm)
        {
            leftOperand = BuildAndAnalyzeTerm(symbolTable, leftTerm);
            comment.Append(leftTerm);
        }
        else
        {
            AnalyzeExpression(symbolTable, EILRegister.RTmp1, expression.LeftExpression);
            leftOperand = new ILRegisterOperand(EILRegister.RTmp1, byteSize);
            comment.Append(EILRegister.RTmp1);
        }
        
        if (expression.RightExpression is AstTerm rightTerm)
        {
            rightOperand = BuildAndAnalyzeTerm(symbolTable, rightTerm);
            comment.Append(" ").Append(expression.Operation.Operation.AsString()).Append(" ").Append(rightTerm);
        }
        else
        {
            AnalyzeExpression(symbolTable, EILRegister.RTmp2, expression.RightExpression);
            rightOperand = new ILRegisterOperand(EILRegister.RTmp2, byteSize);
            comment.Append(" ").Append(expression.Operation.Operation.AsString()).Append(" ").Append(EILRegister.RTmp2);
        }

        switch (expression.Operation.Operation)
        {
            case EBinaryOperator.Addition:
                if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer))
                    _instructions.Addi(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer))
                    _instructions.Addu(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real))
                    _instructions.Addf(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else
                    throw new Exception("Cannot emit add instruction: type values not congruent");
                break;
                
            case EBinaryOperator.Subtraction:
                if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer))
                    _instructions.Subi(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer))
                    _instructions.Subu(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real))
                    _instructions.Subf(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else
                    throw new Exception("Cannot emit sub instruction: type values not congruent");
                break;
                
            case EBinaryOperator.Multiplication:
                if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer))
                    _instructions.Muli(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer))
                    _instructions.Mulu(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real))
                    _instructions.Mulf(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else
                    throw new Exception("Cannot emit sub instruction: type values not congruent");
                break;
                
            case EBinaryOperator.Division:
                if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed | ETypeFlags.Integer))
                    _instructions.Divi(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Integer))
                    _instructions.Divu(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else if (expression.LeftExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real) && expression.RightExpression.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Real))
                    _instructions.Divf(destinationRegister, leftOperand, rightOperand, byteSize, comment.ToString());
                else
                    throw new Exception("Cannot emit sub instruction: type values not congruent");
                break;
                
            default:
                throw new Exception($"Cannot generate intermediate instruction for operator: '{expression.Operation.Operation.AsString()}'");
        }
    }

    public ILOperand BuildAndAnalyzeTerm(SymbolTable symbolTable, AstTerm term)
    {
        switch (term)
        {
            case AstIntegerLiteralTerm intTerm:
                return new ILImmediateIntegerValueOperand(term.TypeInformation.TypeFlags.HasFlag(ETypeFlags.Signed), term.TypeInformation.ByteSize, intTerm.IntegerLiteral.Literal);
            
            case AstFloatLiteralTerm floatTerm:
                return new ILImmediateFloatValueOperand(term.TypeInformation.ByteSize, floatTerm.FloatLiteral.Literal);
            
            // case AstStringLiteralTerm stringTerm:
            //     return new ILStringLiteralTerm(_primitiveSt, stringTerm.StringLiteral.Literal);
            
            case AstVariableDereferenceTerm variableTerm:
                var symbolInfo = symbolTable.FindSymbol(variableTerm.VariableIdentifier.Literal);
                return new ILRelativeAddressOperand(EILRegister.RSP, symbolInfo.StackOffset, symbolInfo.TypeInformation.ByteSize);
            
            default:
                throw new Exception($"Cannot build term: unrecognized term type: {term}");
        }
    }
}