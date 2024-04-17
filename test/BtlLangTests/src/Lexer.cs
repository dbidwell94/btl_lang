using BtlLang;
using BtlLang.Tokens;

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
        Assert.Equal("(", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal(")", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal(".", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal(";", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal(",", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("{", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("}", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("[", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("]", ((Punctuation)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void TestDualCharacterOperators()
    {
        var lexer = new Lexer("== != <= >= && ||");
        var tokens = lexer.Lex(true);
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("==", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("!=", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("<=", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal(">=", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("&&", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("||", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void TestSingleCharacterOperators()
    {
        var lexer = new Lexer("+-*/%&|~^<>");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("+", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("-", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("*", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("/", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("%", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("&", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("|", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("~", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("^", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("<", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal(">", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());

    }

    [Fact]
    public void TestComments()
    {
        var lexer = new Lexer("// This is a comment");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("// This is a comment", ((Comment)enumerator.Current.Type).Value);
    }

    [Fact]
    public void TestStrings()
    {
        var lexer = new Lexer("'H'");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("'H'", ((Literal)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void IgnoreWhitespace()
    {
        var lexer = new Lexer("    ");
        var tokens = lexer.Lex(true);
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void TestIdentifier()
    {
        var lexer = new Lexer("foo");
        var tokens = lexer.Lex();
        var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("foo", ((Identifier)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }
}