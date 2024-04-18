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

        using var enumerator = tokens.GetEnumerator();
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
        using var enumerator = tokens.GetEnumerator();
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
        var lexer = new Lexer("+-*/%&|~^<> = !");
        var tokens = lexer.Lex(true);
        using var enumerator = tokens.GetEnumerator();
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
        Assert.Equal("=", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("!", ((Operator)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());

    }

    [Fact]
    public void TestComments()
    {
        var lexer = new Lexer("// This is a comment");
        var tokens = lexer.Lex();
        using var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("// This is a comment", ((Comment)enumerator.Current.Type).Value);
    }

    [Fact]
    public void TestStrings()
    {
        var lexer = new Lexer("'H'");
        var tokens = lexer.Lex();
        using var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("'H'", ((Literal)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void TestWhitespace()
    {
        var lexer = new Lexer(" \t\n");
        var tokens = lexer.Lex();
        using var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsWhitespace());
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsWhitespace());
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsWhitespace());
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void TestIgnoreWhitespace()
    {
        var lexer = new Lexer("    ");
        var tokens = lexer.Lex(true);
        using var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void TestIdentifierNoUnderscore()
    {
        var lexer = new Lexer("foo");
        var tokens = lexer.Lex();
        using var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal("foo", ((Identifier)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }

    [Fact]
    public void TestIdentifierWithUnderscores()
    {
        var lexer = new Lexer("__foo_bar__");
        var tokens = lexer.Lex();
        using (var enumerator = tokens.GetEnumerator())
        {
            enumerator.MoveNext();
            Assert.Equal("__foo_bar__", ((Identifier)enumerator.Current.Type).Value);
            enumerator.MoveNext();
            Assert.True(enumerator.Current.Type.IsEndOfFile()); 
        }

        // Ensure that identifiers don't prematurely get counted as a keyword
        lexer = new Lexer("ifreturnelsewhileforreturn");
        tokens = lexer.Lex();
        using (var enumerator = tokens.GetEnumerator())
        {
            enumerator.MoveNext();
            Assert.Equal("ifreturnelsewhileforreturn", ((Identifier)enumerator.Current.Type).Value);
        }
    }

    [Fact]
    public void TestKeywords()
    {
        var lexer = new Lexer("if else while for return");
        var tokens = lexer.Lex(true);
        using var enumerator = tokens.GetEnumerator();
        
        enumerator.MoveNext();
        Assert.Equal("if", ((Keyword)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("else", ((Keyword)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("while", ((Keyword)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("for", ((Keyword)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.Equal("return", ((Keyword)enumerator.Current.Type).Value);
        enumerator.MoveNext();
        Assert.True(enumerator.Current.Type.IsEndOfFile());
    }
    
    [Fact]
    public void TestNumberLiteral()
    {
        var lexer = new Lexer("123");
        var tokens = lexer.Lex();
        using (var enumerator = tokens.GetEnumerator())
        {
            enumerator.MoveNext();
            Assert.Equal("123", ((Literal)enumerator.Current.Type).Value);
        }

        lexer = new Lexer("123.456");
        tokens = lexer.Lex();
        using (var enumerator = tokens.GetEnumerator())
        {
            enumerator.MoveNext();
            Assert.Equal("123.456", ((Literal)enumerator.Current.Type).Value);
        }

        lexer = new Lexer(".123");
        tokens = lexer.Lex();
        using (var enumerator = tokens.GetEnumerator())
        {
            enumerator.MoveNext();
            Assert.Equal(".123", ((Literal)enumerator.Current.Type).Value);
        }
    }
}
