using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PGL.Core;

namespace PGL.Frontend;

public class Lexer
{
    public char Current => _fileContents[_index];
    public char Previous => _index - 1 < _fileContents.Length ? _fileContents[_index - 1] : '\0';
    public char Peek => _index + 1 < _fileContents.Length ? _fileContents[_index + 1] : '\0';

    private readonly ILogger _logger;
    private readonly string _sourceFile;
    private readonly string _fileContents;
    private readonly List<Token> _tokens;

    private int _index;
    private uint _currentColumn;
    private uint _currentLine;

    public Lexer(ILogger logger, string sourceFile, string fileContents)
    {
        _logger = logger;
        _sourceFile = sourceFile;
        _fileContents = fileContents;
        _index = 0;
        _currentColumn = 1;
        _currentLine = 1;
        _tokens = new List<Token>();
    }

    /// <summary>
    /// Whether or not the index is a whitespace character.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsWhiteSpace() => Char.IsWhiteSpace(Current);

    /// <summary>
    /// Whether or not the index is a letter character. 
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsLetter() => Char.IsLetter(Current);

    /// <summary>
    /// Whether or not the index is a digit character.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsDigit() => Char.IsDigit(Current);

    /// <summary>
    /// Whether or not the index is a quote character.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsStringLiteral() => Current == '"';

    public void EatChar()
    {
        _index++;
        _currentColumn++;
        if (Previous == '\n')
        {
            _currentLine++;
            _currentColumn = 1;
        }
    }

    public void EatWhitespace()
    {
        while (IsWhiteSpace()) EatChar();
    }

    public void EatLine()
    {
        while (_index < _fileContents.Length && Current != '\n') EatChar();
        if (_index < _fileContents.Length) EatChar();
    }

    public void EatBlockComment()
    {
        EatChar();
        for (;;)
        {
            EatChar();
            if (Current == '*' && Peek == '/')
            {
                EatChar();
                EatChar();
                return;
            }
        }
    }

    public void ReadNumericLiteral()
    {
        var col = _currentColumn;
        var start = _index;
        var decimalFound = false;

        for (;;)
        {
            if (Current == '.')
            {
                if (decimalFound) break;
                decimalFound = true;
            }
            else if (!IsDigit()) break;

            EatChar();
        }

        var lit = _fileContents.Substring(start, _index - start);
        PushToken(decimalFound ? ETokenType.LiteralFloat : ETokenType.LiteralInteger, _currentLine, col, lit);
    }

    [SuppressMessage("Reliability", "CA2014:Do not use stackalloc in loops")]
    public void ReadStringLiteral()
    {
        EatChar(); // Eat the opening "
        var i = 0;
        var c = _fileContents[_index];
        while (c != '"')
        {
            c = _fileContents[_index + i];
            if (c == '\n')
            {
                Error("Attempted to read a string literal that wrapped the line.");
                /* todo ahodder@praethos.com 9/28/23: We should not throw an error here: instead handle gracefully */
                throw new Exception("String literal overflow");
            }

            i++;
        }
        
        PushToken(ETokenType.LiteralString, _fileContents.Substring(_index, i - 1));
        _index += i;
    }

    public ETokenType CheckForKeyword(string ident)
    {
        switch (ident)
        {
            case "fn": return ETokenType.KeywordFn;
            case "if": return ETokenType.KeywordIf;
            case "elif": return ETokenType.KeywordElif;
            case "else": return ETokenType.KeywordElse;
            case "return": return ETokenType.KeywordReturn;
            
            case "int": return ETokenType.KeywordTypeInt;
            case "i8": return ETokenType.KeywordTypeI8;
            case "i16": return ETokenType.KeywordTypeI16;
            case "i32": return ETokenType.KeywordTypeI32;
            case "i64": return ETokenType.KeywordTypeI64;
                
            case "uint": return ETokenType.KeywordTypeUint;
            case "u8": return ETokenType.KeywordTypeU8;
            case "u16": return ETokenType.KeywordTypeU16;
            case "u32": return ETokenType.KeywordTypeU32;
            case "u64": return ETokenType.KeywordTypeU64;
            
            case "float": return ETokenType.KeywordTypeFloat;
            case "f32": return ETokenType.KeywordTypeF32;
            case "f64": return ETokenType.KeywordTypeF64;
            
            case "true": return ETokenType.KeywordTypeTrue;
            case "false": return ETokenType.KeywordTypeFalse;

            default: return ETokenType.Identifier;
        }
    }

    public void ReadIdentifier()
    {
        var col = _currentColumn;
        var start = _index;

        while (IsLetter() || IsDigit() || Current == '_')
        {
            EatChar();
        }

        var ident = _fileContents.Substring(start, _index - start);
        PushToken(CheckForKeyword(ident), _currentLine, col, ident);
    }
    
    public void PushToken(ETokenType type, string literal = null!)
    {
        PushToken(type, _currentLine, _currentColumn, literal);
    }

    public void PushToken(ETokenType type, uint line, uint column, string literal = null!)
    {
        var token = new Token(_sourceFile, line, column, type, literal);
        _tokens.Add(token);
    }

    public void EatAndPushToken(ETokenType type)
    {
        PushToken(type, $"{Current}");
        EatChar();
    }

    public List<Token> Tokenize()
    {
        PushToken(ETokenType.File, 0, 0, _sourceFile);
        while (_index < _fileContents.Length)
        {
            EatWhitespace();

            if (IsLetter())
            {
                ReadIdentifier();
                continue;
            }

            if (IsDigit())
            {
                ReadNumericLiteral();
                continue;
            }

            if (IsStringLiteral())
            {
                ReadStringLiteral();
                continue;
            }
                

            // Fallback on the common tokens
            switch (Current)
            {
                case '+':
                    EatAndPushToken(ETokenType.SymbolPlus);
                    continue;

                case '-':
                    EatAndPushToken(ETokenType.SymbolHyphen);
                    continue;

                case '*':
                    EatAndPushToken(ETokenType.SymbolStar);
                    continue;

                case '/':
                    if (Peek == '/')
                    {
                        EatLine();
                    }
                    else if (Peek == '*')
                    {
                        EatBlockComment();
                    }
                    else
                    {
                        EatAndPushToken(ETokenType.SymbolForwardSlash);
                    }

                    continue;

                case '\\':
                    EatAndPushToken(ETokenType.SymbolBackSlash);
                    continue;

                case '=':
                    EatAndPushToken(ETokenType.SymbolEquals);
                    continue;

                case '|':
                    EatAndPushToken(ETokenType.SymbolVerticalPipe);
                    continue;

                case '!':
                    EatAndPushToken(ETokenType.SymbolExclamation);
                    continue;

                case '@':
                    EatAndPushToken(ETokenType.SymbolAt);
                    continue;

                case '#':
                    EatAndPushToken(ETokenType.SymbolHash);
                    continue;

                case '$':
                    EatAndPushToken(ETokenType.SymbolDollar);
                    continue;

                case '%':
                    EatAndPushToken(ETokenType.SymbolModulus);
                    continue;

                case '^':
                    EatAndPushToken(ETokenType.SymbolCarrot);
                    continue;

                case '&':
                    EatAndPushToken(ETokenType.SymbolAmpersand);
                    continue;

                case ',':
                    EatAndPushToken(ETokenType.SymbolComma);
                    continue;

                case '.':
                    EatAndPushToken(ETokenType.SymbolPeriod);
                    continue;

                case ':':
                    EatAndPushToken(ETokenType.SymbolColon);
                    continue;

                case ';':
                    EatAndPushToken(ETokenType.SymbolSemiColon);
                    continue;

                case '(':
                    EatAndPushToken(ETokenType.SymbolLeftParen);
                    continue;

                case ')':
                    EatAndPushToken(ETokenType.SymbolRightParen);
                    continue;

                case '[':
                    EatAndPushToken(ETokenType.SymbolLeftSquare);
                    continue;

                case ']':
                    EatAndPushToken(ETokenType.SymbolRightSquare);
                    continue;

                case '{':
                    EatAndPushToken(ETokenType.SymbolLeftCurly);
                    continue;

                case '}':
                    EatAndPushToken(ETokenType.SymbolRightCurly);
                    continue;

                case '<':
                    EatAndPushToken(ETokenType.SymbolLesserThan);
                    break;

                case '>':
                    EatAndPushToken(ETokenType.SymbolGreaterThan);
                    break;

                default:
                    Error($"Unexpected token: {Current}");
                    break;
            }
        }

        return new List<Token>(_tokens);
    }

    private void Error(string message)
    {
        _logger.LexerError(_sourceFile, _currentLine, _currentColumn, message);
    }
}