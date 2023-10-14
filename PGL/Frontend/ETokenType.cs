namespace PGL.Frontend;

public enum ETokenType
{
    Unknown,
    File,
    Eof,
    
    Identifier,
    
    LiteralInteger,
    LiteralFloat,
    LiteralString,
    
    KeywordTypeInt,
    KeywordTypeI8,
    KeywordTypeI16,
    KeywordTypeI32,
    KeywordTypeI64,
    
    KeywordTypeUint,
    KeywordTypeU8,
    KeywordTypeU16,
    KeywordTypeU32,
    KeywordTypeU64,
    
    KeywordTypeFloat,
    KeywordTypeF32,
    KeywordTypeF64,
    
    KeywordTypeBool,
    
    KeywordTrue,
    KeywordFalse,
    KeywordFn,
    
    KeywordIf,
    KeywordElif,
    KeywordElse,
    
    KeywordReturn,
    
    SymbolPlus,
    SymbolHyphen,
    SymbolStar,
    SymbolForwardSlash,
    SymbolModulus,

    SymbolEquals,
    SymbolVerticalPipe,
    SymbolGreaterThan,
    SymbolLesserThan,
    
    SymbolBackSlash,
    SymbolExclamation,
    SymbolAt,
    SymbolHash,
    SymbolDollar,
    SymbolCarrot,
    SymbolAmpersand,
        
    SymbolComma,
    SymbolPeriod,
    SymbolColon,
    SymbolSemiColon,
    SymbolSingleQuote,
    SymbolDoubleQuote,
    
    SymbolLeftParen,
    SymbolRightParen,
    SymbolLeftSquare,
    SymbolRightSquare,
    SymbolLeftCurly,
    SymbolRightCurly,
}

public static class TokenTypeExtension
{
    public static bool IsKeywordType(this ETokenType self)
    {
        switch (self)
        {
            case ETokenType.KeywordTypeInt:
            case ETokenType.KeywordTypeI8:
            case ETokenType.KeywordTypeI16:
            case ETokenType.KeywordTypeI32:
            case ETokenType.KeywordTypeI64:
                
            case ETokenType.KeywordTypeUint:
            case ETokenType.KeywordTypeU8:
            case ETokenType.KeywordTypeU16:
            case ETokenType.KeywordTypeU32:
            case ETokenType.KeywordTypeU64:
                
            case ETokenType.KeywordTypeFloat:
            case ETokenType.KeywordTypeF32:
            case ETokenType.KeywordTypeF64:
                
            case ETokenType.KeywordTypeBool:
                return true;
            
            default: return false;
        }
    }
    
    public static bool IsBinaryOperator(this ETokenType self)
    {
        switch (self)
        {
            case ETokenType.SymbolPlus:
            case ETokenType.SymbolHyphen:
            case ETokenType.SymbolStar:
            case ETokenType.SymbolForwardSlash:
                return true;
                
            default: return false;
        }
    }
}