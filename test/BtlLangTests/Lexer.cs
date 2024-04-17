using BtlLang;

namespace BtlLangTests;

public class LexerTest
{
    [Fact]
    public void TestPunctuation()
    {
        var lexer = new Lexer("().;,{}[]");
        var tokens = lexer.Lex();

        var enumerator = tokens.GetEnumerator();

        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal("(", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal(")", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal(".", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal(";", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal(",", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal("{", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal("}", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal("[", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Punctuation, enumerator.Current.Type);
        Assert.Equal("]", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.EndOfFile, enumerator.Current.Type);
    }

    [Fact]
    public void TestDualCharacterOperators()
    {
        var lexer = new Lexer("== != <= >= && ||");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("==", enumerator.Current.Value);
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("!=", enumerator.Current.Value);
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("<=", enumerator.Current.Value);
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal(">=", enumerator.Current.Value);
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("&&", enumerator.Current.Value);
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("||", enumerator.Current.Value);
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal(TokenType.EndOfFile, enumerator.Current.Type);
    }

    [Fact]
    public void TestSingleCharacterOperators()
    {
        var lexer = new Lexer("+-*/%&|~^<>");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("+", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("-", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("*", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("/", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("%", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("&", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("|", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("~", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("^", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal("<", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.Operator, enumerator.Current.Type);
        Assert.Equal(">", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.EndOfFile, enumerator.Current.Type);
    }

    [Fact]
    public void TestComments() {
        var lexer = new Lexer("// This is a comment");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Comment, enumerator.Current.Type);
        Assert.Equal("// This is a comment", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.EndOfFile, enumerator.Current.Type);
    }

    [Fact]
    public void TestStrings()
    {
        var lexer = new Lexer("'H'");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Literal, enumerator.Current.Type);
        Assert.Equal("'H'", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.EndOfFile, enumerator.Current.Type);
    }

    [Fact]
    public void IgnoreWhitespace()
    {
        var lexer = new Lexer("    ");
        var tokens = lexer.Lex(true);
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal(TokenType.EndOfFile, enumerator.Current.Type);
    }

    [Fact]
    public void TestIdentifier()
    {
        var lexer = new Lexer("foo");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal(TokenType.Identifier, enumerator.Current.Type);
        Assert.Equal("foo", enumerator.Current.Value);
        enumerator.MoveNext();
        Assert.Equal(TokenType.EndOfFile, enumerator.Current.Type);
    }
}