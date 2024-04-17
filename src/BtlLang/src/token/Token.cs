namespace BtlLang.Tokens;

public abstract record TokenType;

public record Identifier(string Value) : TokenType;

public record Type(string Value) : TokenType;

public record Operator(string Value) : TokenType;

public record Literal(string Value) : TokenType;

public record Keyword(string Value) : TokenType;

public record Punctuation(string Value) : TokenType;

public record Comment(string Value) : TokenType;

public record Whitespace : TokenType;

public record Newline : TokenType;

public record EndOfFile : TokenType;

public readonly struct Token
{
    public TokenType Type { get; }
    public int Line { get; }
    public int Column { get; }

    public Token(TokenType type, int line, int column)
    {
        Type = type;
        Line = line;
        Column = column;
    }
}