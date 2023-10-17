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
        AnalyzeProgram(_program);
        return true;
    }

    public void AnalyzeProgram(AstProgram program)
    {
        foreach (var function in program.Functions)
        {
            AnalyzeFunction(program, function);
        }
    }

    public void AnalyzeFunction(AstProgram program, AstFunction function)
    {
        function.ArgsAndRetsSymbolTable = new SymbolTable();
        /* todo ahodder@praethos.com 10/9/23: we need to ensure that all code paths return */
        for (var i = 0; i < function.ReturnArguments.Count; i++)
        {
            var decl = function.ReturnArguments[i];
            decl.TypeInformation = program.TypeTable.FindType(decl.TypeIdentifier.Literal);
            if (decl.VariableIdentifier == null)
                decl.VariableIdentifier = new Token(null, 0, 0, ETokenType.Identifier, $"ret{i}");
            function.ArgsAndRetsSymbolTable.RegisterSymbolWithType(decl.VariableIdentifier.Literal, decl.TypeInformation);
        }

        function.Statements.SymbolTable = function.ArgsAndRetsSymbolTable.CreateSubTable();

        foreach (var statement in function.Statements.Statements)
        {
            AnalyzeStatement(program, function, statement);
        }
    }

    public void AnalyzeStatement(AstProgram program, AstFunction function, AstStatement statement)
    {
        switch (statement)
        {
            case AstReturnStatement returnStatement:
                DetermineExpressionTypeAssignment(program.TypeTable, function.Statements.SymbolTable, returnStatement.Expression);
                EnsureTypesAreEqual(returnStatement.Expression.TypeInformation, function.ReturnArguments[0].TypeInformation);
                break;
            
            case AstVariableAssignmentStatement assignmentStatement:
                DetermineExpressionTypeAssignment(program.TypeTable, function.Statements.SymbolTable, assignmentStatement.Expression);
                // Check for implicit typing.
                if (assignmentStatement.TypeIdentifierInformation != null)
                    EnsureTypesAreEqual(assignmentStatement.Expression.TypeInformation, program.TypeTable.FindType(assignmentStatement.TypeIdentifierInformation.Literal));
                function.Statements.SymbolTable.RegisterSymbolWithType(assignmentStatement.VariableIdentifierIdentifier.Literal, assignmentStatement.Expression.TypeInformation);
                break;
                
            default:
                throw new Exception($"Cannot type check statement: {statement}");
        }
    }

    public void DetermineExpressionTypeAssignment(TypeTable typeTable, SymbolTable symbolTable, AstExpression expression)
    {
        switch (expression)
        {
            case AstBinaryExpression binaryExpression:
                DetermineExpressionTypeAssignment(typeTable, symbolTable, binaryExpression.LeftExpression);
                DetermineExpressionTypeAssignment(typeTable, symbolTable, binaryExpression.RightExpression);
                EnsureTypesAreEqual(binaryExpression.LeftExpression.TypeInformation, binaryExpression.RightExpression.TypeInformation);
                binaryExpression.TypeInformation = binaryExpression.LeftExpression.TypeInformation;
                break;
            
            case AstTerm termExpression:
                DetermineTermTypeAssignment(typeTable, symbolTable, termExpression);
                break;

            default:
                throw new Exception($"Cannot perform expression type assignment: {expression}");
        }
    }

    public void DetermineTermTypeAssignment(TypeTable typeTable, SymbolTable symbolTable, AstTerm term)
    {
        switch (term)
        {
            case AstIntegerLiteralTerm intTerm:
                var bigInt = BigInteger.Parse(intTerm.IntegerLiteral.Literal);
                var signed = bigInt.Sign;
                var bitLength = bigInt.GetBitLength();
                if (bitLength <= 8)
                    intTerm.TypeInformation = signed >= 0 ? typeTable.FindType(PglType.PrimitiveI8.Symbol) : typeTable.FindType(PglType.PrimitiveU8.Symbol);
                else if (bitLength <= 16)
                    intTerm.TypeInformation = signed >= 0 ? typeTable.FindType(PglType.PrimitiveI16.Symbol) : typeTable.FindType(PglType.PrimitiveU16.Symbol);
                else if (bitLength <= 32)
                    intTerm.TypeInformation = signed >= 0 ? typeTable.FindType(PglType.PrimitiveI32.Symbol) : typeTable.FindType(PglType.PrimitiveU32.Symbol);
                else if (bitLength <= 64)
                    intTerm.TypeInformation = signed >= 0 ? typeTable.FindType(PglType.PrimitiveI64.Symbol) : typeTable.FindType(PglType.PrimitiveU64.Symbol);
                else
                    throw new Exception("COMPILER ERROR: Failed to determine integer bit sizing!");
                break;
            
            case AstFloatLiteralTerm floatTerm:
                if (float.TryParse(floatTerm.FloatLiteral.Literal, out var f))
                    floatTerm.TypeInformation = typeTable.FindType(PglType.PrimitiveF32.Symbol);
                else
                    floatTerm.TypeInformation = typeTable.FindType(PglType.PrimitiveF64.Symbol);
                break;
            
            case AstVariableDereferenceTerm variableDereferenceTerm:
                variableDereferenceTerm.TypeInformation = symbolTable.FindSymbol(variableDereferenceTerm.VariableIdentifier.Literal).TypeInformation;
                break;
            
            default:
                throw new Exception($"Cannot type assign term: {term}");
        }
    }

    public void EnsureTypesAreEqual(AstTypeInformation type1, AstTypeInformation type2)
    {
        if (type1 == null || type2 == null || type1 != type2)
            throw new Exception($"Type {type1} is not equal to {type2}");
    }
}