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

public static class TokenExtensions
{
    public static bool IsIdentifier(this TokenType type) => type is Identifier;
    public static bool IsType(this TokenType type) => type is Type;
    public static bool IsOperator(this TokenType type) => type is Operator;
    public static bool IsLiteral(this TokenType type) => type is Literal;
    public static bool IsKeyword(this TokenType type) => type is Keyword;
    public static bool IsPunctuation(this TokenType type) => type is Punctuation;
    public static bool IsComment(this TokenType type) => type is Comment;
    public static bool IsWhitespace(this TokenType type) => type is Whitespace;
    public static bool IsNewline(this TokenType type) => type is Newline;
    public static bool IsEndOfFile(this TokenType type) => type is EndOfFile;


    public static string GetTypeName(this TokenType type) => type switch
    {
        Identifier => "Identifier",
        Type => "Type",
        Operator => "Operator",
        Literal => "Literal",
        Keyword => "Keyword",
        Punctuation => "Punctuation",
        Comment => "Comment",
        Whitespace => "Whitespace",
        Newline => "Newline",
        EndOfFile => "EndOfFile",
        _ => "Unknown"
    };
}