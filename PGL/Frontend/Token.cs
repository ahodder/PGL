namespace PGL.Frontend;

public struct Token
{
    public string SourceFile { get; }
    public uint Line { get; }
    public uint Column { get; }
    public ETokenType Type { get; }
    public string Literal { get; }
    
    public Token(string sourceFile, uint line, uint column, ETokenType type, string literal = null!)
    {
        SourceFile = sourceFile;
        Line = line;
        Column = column;
        Type = type;
        Literal = literal;
    }

    public bool Is(string literal) => literal.Equals(Literal);

    public override string ToString() => Literal != null ? $"Token [{Type}] -> {Literal}" : $"Token [{Type}]";
}