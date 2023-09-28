namespace PGL.Frontend;

public enum ETokenType
{
    Unknown,
    File,
    Eof,
    
    Identifier,
    
    LiteralInteger,
    LiteralFractional,
    LiteralString,
    
    KeywordInt,
    KeywordI8,
    KeywordI16,
    KeywordI32,
    KeywordI64,
    
    KeywordUint,
    KeywordU8,
    KeywordU16,
    KeywordU32,
    KeywordU64,
    
    KeywordFloat,
    KeywordF32,
    KeywordF64,
    
    KeywordTrue,
    KeywordFalse,
    
    KeywordIf,
    KeywordElif,
    KeywordElse,
    
    KeywordReturn,
    
    SymbolPlus,
    SymbolHyphen,
    SymbolStar,
    SymbolModulus,
    SymbolForwardSlash,
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
    
    SymbolLeftParen,
    SymbolRightParen,
    SymbolLeftSquare,
    SymbolRightSquare,
    SymbolLeftCurly,
    SymbolRightCurly,
}