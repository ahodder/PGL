using System.Numerics;
using PGL.Ast;
using PGL.Core;

namespace PGL.Frontend;

public class SemanticAnalysis
{
    private Configuration _configuration;
    private AstProgram _program;

    public SemanticAnalysis(Configuration configuration, AstProgram program)
    {
        _configuration = configuration;
        _program = program;
    }

    public bool Analyze()
    {
        // Initial type checking
        PerformProgramTypeAssignment(_program);
        // PerformAstTypeAssignment(_program);
        return true;
    }

    public void PerformProgramTypeAssignment(AstProgram program)
    {
        program.SymbolTable = SymbolTable.CreateRootSymbolTable(_configuration);
        foreach (var function in program.Functions)
        {
            function.SymbolTable = program.SymbolTable.CreateSubSymbolTable();
            DetermineFunctionTypeAssignment(function);
            // The statement types need to be present before we can returns can be checked.
            PerformFunctionReturnTypeChecking(function);
        }
    }

    public void PerformFunctionReturnTypeChecking(AstFunction function)
    {
        /* todo ahodder@praethos.com 10/9/23: we need to ensure that all code paths return */

        foreach (var ret in function.ReturnArguments)
        {
            ret.ResolvedType = function.SymbolTable.FindType(ret.TypeIdentifier.Identifier.Literal);
        }

        foreach (var statement in function.Statements.Statements)
        {
            if (statement is AstReturnStatement returnStatement)
            {
                EnsureTypesAreEqual(returnStatement.Expression.ResolvedType, function.ReturnArguments[0].ResolvedType);
            }
        }
    }

    public void DetermineFunctionTypeAssignment(AstFunction function)
    {
        foreach (var statement in function.Statements.Statements)
        {
            DetermineStatementTypeAssignment(function.SymbolTable, statement);
        }
    }

    public void DetermineStatementTypeAssignment(SymbolTable symbolTable, AstStatement statement)
    {
        switch (statement)
        {
            case AstReturnStatement returnStatement:
                DetermineExpressionTypeAssignment(symbolTable, returnStatement.Expression);
                break;
            
            case AstVariableAssignmentStatement assignmentStatement:
                DetermineVariableTypeAssignment(symbolTable, assignmentStatement);
                symbolTable.RegisterSymbolWithType(assignmentStatement.VariableIdentifier.Literal, assignmentStatement.Expression.ResolvedType);
                break;
                
            default:
                throw new Exception($"Cannot type check statement: {statement}");
        }
    }

    public void DetermineExpressionTypeAssignment(SymbolTable symbolTable, AstExpression expression)
    {
        switch (expression)
        {
            case AstBinaryExpression binaryExpression:
                DetermineExpressionTypeAssignment(symbolTable, binaryExpression.LeftExpression);
                DetermineExpressionTypeAssignment(symbolTable, binaryExpression.RightExpression);
                EnsureTypesAreEqual(binaryExpression.LeftExpression.ResolvedType, binaryExpression.RightExpression.ResolvedType);
                binaryExpression.ResolvedType = binaryExpression.LeftExpression.ResolvedType;
                break;
            
            case AstTerm termExpression:
                DetermineTermTypeAssignment(symbolTable, termExpression);
                break;

            default:
                throw new Exception($"Cannot perform expression type assignment: {expression}");
        }
    }

    public void DetermineVariableTypeAssignment(SymbolTable symbolTable, AstVariableAssignmentStatement statement)
    {
        DetermineExpressionTypeAssignment(symbolTable, statement.Expression);
        // Check for implicit typing.
        if (statement.TypeIdentifier?.ResolvedType != null)
        {
            EnsureTypesAreEqual(statement.Expression.ResolvedType, statement.TypeIdentifier.ResolvedType);
        }
        else
        {
            statement.TypeIdentifier = new AstTypeIdentifier(default)
            {
                ResolvedType = statement.Expression.ResolvedType,
            };
        }
    }

    public void DetermineTermTypeAssignment(SymbolTable symbolTable, AstTerm term)
    {
        switch (term)
        {
            case AstIntegerLiteralTerm intTerm:
                var bigInt = BigInteger.Parse(intTerm.IntegerLiteral.Literal);
                var signed = bigInt.Sign;
                var bitLength = bigInt.GetBitLength();
                if (bitLength <= 8)
                    intTerm.ResolvedType = signed >= 0 ? PglType.PrimitiveI8 : PglType.PrimitiveU8;
                else if (bitLength <= 16)
                    intTerm.ResolvedType = signed >= 0 ? PglType.PrimitiveI16 : PglType.PrimitiveU16;
                else if (bitLength <= 32)
                    intTerm.ResolvedType = signed >= 0 ? PglType.PrimitiveI32 : PglType.PrimitiveU32;
                else if (bitLength <= 64)
                    intTerm.ResolvedType = signed >= 0 ? PglType.PrimitiveI64 : PglType.PrimitiveU64;
                break;
            
            case AstFloatLiteralTerm floatTerm:
                if (float.TryParse(floatTerm.FloatLiteral.Literal, out var f))
                    floatTerm.ResolvedType = PglType.PrimitiveF32;
                else
                    floatTerm.ResolvedType = PglType.PrimitiveF64;
                break;
            
            case AstVariableDereferenceTerm variableDereferenceTerm:
                variableDereferenceTerm.ResolvedType = symbolTable.FindType(variableDereferenceTerm.VariableIdentifier.Literal);
                break;
            
            default:
                throw new Exception($"Cannot type assign term: {term}");
        }
    }

    public void EnsureTypesAreEqual(PglType type1, PglType type2)
    {
        if (type1 == null || type2 == null || type1 != type2)
            throw new Exception($"Type {type1} is not equal to {type2}");
    }
}