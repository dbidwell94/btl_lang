using BtlLang.Tokens;

namespace BtlLangTests.Tokens;

public class TokenTests
{
    [Fact]
    public void TestTokenEquality()
    {
        var token1 = new Token(new Identifier("test"), 1, 1);
        var token2 = new Token(new Identifier("test"), 1, 1);
        Assert.Equal(token1, token2);
    }

    [Fact]
    public void TestTokenInequality()
    {
        var token1 = new Token(new Identifier("test"), 1, 1);
        var token2 = new Token(new Identifier("test"), 1, 2);
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void TestExtensions()
    {
        var token = new Token(new Identifier("test"), 1, 1);
        Assert.True(token.Type.IsIdentifier());
        Assert.False(token.Type.IsOperator());
        Assert.False(token.Type.IsLiteral());
        Assert.False(token.Type.IsKeyword());
        Assert.False(token.Type.IsPunctuation());
        Assert.False(token.Type.IsComment());
        Assert.False(token.Type.IsWhitespace());
        Assert.False(token.Type.IsEndOfFile());
        Assert.Equal("Identifier", token.Type.GetTypeName());
    }
}