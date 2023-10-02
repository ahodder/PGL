using PGL.Ast;

namespace PGL.Frontend;

public class Parser
{
    private bool HasNextToken => _index < _tokens.Count - 1;
    private Token CurrentToken => _tokens[_index];
    private Token PeekToken => _tokens[_index + 1];
    private Token EatToken() => _tokens[_index++];
    
    private List<Token> _tokens;
    private int _index;
    
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }
    
    public IAstNode Parse()
    {
        var file = ExpectToken(ETokenType.File);

        var functions = new List<AstFunction>();

        while (_index < _tokens.Count)
        {
            switch (CurrentToken.Type)
            {
                case ETokenType.KeywordFn:
                    functions.Add(ExpectFunction());
                    break;

                default:
                    throw new Exception($"Cannot parse ast: unexpected token: {CurrentToken}");
            }
        }

        var astFile = new AstFile(file, functions);
        return astFile;
    }

    public AstFunction ExpectFunction()
    {
        ExpectToken(ETokenType.KeywordFn);

        var functionIdentifier = ExpectToken(ETokenType.Identifier);
        var functionArguments = new List<AstVariableTypeDeclaration>();
        Token? equals = null;
        var functionReturns = new List<AstVariableTypeDeclaration>();

        if (CurrentToken.Type == ETokenType.SymbolLeftParen)
        {
            while (CurrentToken.Type != ETokenType.SymbolRightParen)
            {
                functionArguments.Add(ExpectAstVariableTypeDeclaration());
            }
        }

        if (CurrentToken.Type == ETokenType.SymbolEquals)
        {
            equals = ExpectToken(ETokenType.SymbolEquals);
            while (CurrentToken.Type != ETokenType.SymbolLeftCurly)
            {
                // Named return
                if (PeekToken.Type == ETokenType.SymbolColon)
                {
                    functionReturns.Add(ExpectAstVariableTypeDeclaration());
                }
                else
                {
                    var type = ExpectTypeIdentifier();
                    functionReturns.Add(new AstVariableTypeDeclaration(null, type));
                }
            }
        }

        var statementBlock = ExpectStatementBlock();
        
        return new AstFunction(functionIdentifier, functionArguments, functionReturns, statementBlock);
    }

    public AstStatementBlock ExpectStatementBlock()
    {
        ExpectToken(ETokenType.SymbolLeftCurly);

        var statements = new List<AstStatement>();

        while (CurrentToken.Type != ETokenType.SymbolRightCurly)
            statements.Add(ExpectStatement());
        
        ExpectToken(ETokenType.SymbolRightCurly);

        return new AstStatementBlock(statements);
    }

    public AstStatement ExpectStatement()
    {
        if (CurrentToken.Type == ETokenType.KeywordReturn)
        {
            ExpectToken(ETokenType.KeywordReturn);
            var expression = ExpectExpression();
            ExpectToken(ETokenType.SymbolSemiColon);
            return new AstReturnStatement(expression);
        } 
        else if (CurrentToken.Type == ETokenType.Identifier)
        {
            if (PeekToken.Type == ETokenType.SymbolColon)
            {
                var variable = ExpectToken(ETokenType.Identifier);
                ExpectToken(ETokenType.SymbolColon);
                AstTypeIdentifier typeIdentifier = null;
                if (CurrentToken.Type != ETokenType.SymbolEquals)
                    typeIdentifier = ExpectTypeIdentifier();
                ExpectToken(ETokenType.SymbolEquals);
                var expression = ExpectExpression();
                ExpectToken(ETokenType.SymbolSemiColon);
                return new AstVariableAssignmentStatement(variable, typeIdentifier, expression);
            }
            else
            {
                var expression = ExpectFunctionInvocationExpression();
                ExpectToken(ETokenType.SymbolSemiColon);
                return new AstExpressionStatement(expression);
            }
        } 

        throw new Exception($"Cannot resolve statement: {CurrentToken}");
    }

    public AstExpression ExpectExpression()
    {
        // Binary Expression
        if (IsOperator(PeekToken))
            return ExpectBinaryExpression();
        else if (CurrentToken.Type == ETokenType.Identifier)
            return ExpectFunctionInvocationExpression();
        else
            return ExpectTermExpression(); 
    }

    public AstBinaryOperator ExpectBinaryOperator()
    {
        switch (CurrentToken.Type)
        {
            case ETokenType.SymbolPlus: return new AstBinaryOperator(EatToken(), EBinaryOperator.Addition); 
            case ETokenType.SymbolHyphen: return new AstBinaryOperator(EatToken(), EBinaryOperator.Subtraction); 
            case ETokenType.SymbolStar: return new AstBinaryOperator(EatToken(), EBinaryOperator.Multiplication); 
            case ETokenType.SymbolForwardSlash: return new AstBinaryOperator(EatToken(), EBinaryOperator.Division); 
            
            default:
                throw new Exception($"Unexpected binary operator: {CurrentToken}");
        }
    }

    public bool IsArithmaticToken(Token token)
    {
        return token.Type == ETokenType.Identifier ||
               token.Type.IsBinaryOperator() ||
               token.Type == ETokenType.SymbolLeftParen ||
               token.Type == ETokenType.LiteralInteger ||
               token.Type == ETokenType.LiteralFloat;
    }

    public bool IsOperator(Token token)
    {
        switch (token.Type)
        {
            case ETokenType.SymbolPlus:
            case ETokenType.SymbolHyphen:
            case ETokenType.SymbolStar:
            case ETokenType.SymbolForwardSlash:
                return true;
            
            default: return false;
        }
    }

    public AstBinaryExpression ExpectBinaryExpression()
    {
        var terms = new Stack<AstExpression>();
        var operators = new Stack<AstBinaryOperator>();

        while (IsArithmaticToken(CurrentToken))
        {
            if (IsOperator(CurrentToken))
            {
                var op = ExpectBinaryOperator();

                while (operators.Count > 0 && !op.Operation.IsHigherPrecedenceThan(operators.Peek().Operation))
                {
                    var right = terms.Pop();
                    var tmpOp = operators.Pop();
                    var left = terms.Pop();
                    terms.Push(new AstBinaryExpression(left, tmpOp, right));
                }
                
                operators.Push(op);
            }
            else if (CurrentToken.Type == ETokenType.SymbolLeftParen)
            {
                ExpectToken(ETokenType.SymbolLeftParen);
                terms.Push(ExpectBinaryExpression());
                ExpectToken(ETokenType.SymbolRightParen);
            }
            else
                terms.Push(ExpectTermExpression());
        }

        while (terms.Count > 1)
        {
            var right = terms.Pop();
            var op = operators.Pop();
            var left = terms.Pop();
            terms.Push(new AstBinaryExpression(left, op, right));
        }

        var expr = terms.Pop();
        return (AstBinaryExpression)expr;
    }

    public AstTermExpression ExpectTermExpression()
    {
        if (CurrentToken.Type == ETokenType.SymbolHyphen)
            return new AstUnaryNegativeTerm(ExpectExpression());
        if (CurrentToken.Type == ETokenType.LiteralInteger)
            return new AstIntegerLiteralExpression(ExpectToken(ETokenType.LiteralInteger));
        if (CurrentToken.Type == ETokenType.LiteralFloat)
            return new AstFloatLiteralExpression(ExpectToken(ETokenType.LiteralInteger));
        if (CurrentToken.Type == ETokenType.LiteralString)
            return new AstStringLiteralExpression(ExpectToken(ETokenType.LiteralString));
        if (CurrentToken.Type == ETokenType.Identifier)
        {
            if (PeekToken.Type == ETokenType.SymbolLeftParen)
                return ExpectFunctionInvocationExpression();
            else
                return new AstVariableDereferenceTermExpression(ExpectToken(ETokenType.Identifier));
        }

        throw new Exception($"Unexpected term expression: {CurrentToken}");
    }

    public AstFunctionInvocationExpression ExpectFunctionInvocationExpression()
    {
        var functionIdentifier = ExpectToken(ETokenType.Identifier);
        var parameters = new List<AstExpression>();
        ExpectToken(ETokenType.SymbolLeftParen);
        while (CurrentToken.Type != ETokenType.SymbolRightParen)
        {
            parameters.Add(ExpectExpression());
            if (CurrentToken.Type != ETokenType.SymbolRightParen)
                ExpectToken(ETokenType.SymbolComma, "Expected comma between function parameters");
        }
        ExpectToken(ETokenType.SymbolRightParen);

        return new AstFunctionInvocationExpression(functionIdentifier, parameters);
    }

    public AstVariableTypeDeclaration ExpectAstVariableTypeDeclaration()
    {
        return new AstVariableTypeDeclaration(ExpectToken(ETokenType.Identifier), ExpectTypeIdentifier());
    }

    public AstTypeIdentifier ExpectTypeIdentifier()
    {
        if (CurrentToken.Type == ETokenType.Identifier || CurrentToken.Type.IsKeywordType())
            return new AstTypeIdentifier(EatToken());
        throw new Exception($"Expected token of type: (Identifier / keyword type) , but got a token {CurrentToken}");
    }

    public Token ExpectToken(ETokenType tokenType, string errorMessage = null)
    {
        if (CurrentToken.Type != tokenType)
            throw new Exception($"Expected token of type: {tokenType}, but got a token {CurrentToken}\n {errorMessage ?? string.Empty}");

        return EatToken();
    }
}