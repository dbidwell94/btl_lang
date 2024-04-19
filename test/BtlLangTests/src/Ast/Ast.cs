using BtlLang;
using BtlLang.Ast;
using BtlLang.Tokens;

namespace BtlLangTests.Ast;

public class AstParameterTests
{
    [Fact]
    public void TestIsParameter()
    {
        var lexer = new Lexer("(a: int, b: int)");
        var tokens = lexer.Lex(true);
        var seekable = new CachedEnumerable<Token>(tokens);

        ParameterNode nodeToTest;
        
        Assert.True(ParameterNode.IsParameterNode(out nodeToTest, seekable, typeof(FunctionNode)));
        Assert.Equal("a", nodeToTest.Name.Value);
        Assert.Equal("int", nodeToTest.Type.Value);
        Assert.True(ParameterNode.IsParameterNode(out nodeToTest, seekable, typeof(FunctionNode)));
        Assert.Equal("b", nodeToTest.Name.Value);
        Assert.Equal("int", nodeToTest.Type.Value);
    }
    
    [Fact]
    public void TestParameterPunctuationException()
    {
        var lexer = new Lexer("(a: int, b: int");
        var seekable = new CachedEnumerable<Token>(lexer.Lex(true));

        ParameterNode.IsParameterNode(out _, seekable, typeof(FunctionNode));
        Assert.Throws<PunctuationException>(() => ParameterNode.IsParameterNode(out _, seekable, typeof(FunctionNode)));
    }
}

public class AstFunctionTests
{
    [Fact]
    public void TestFunctionAst()
    {
        const string function = """
                                  func add(a: int, b: int): int {
                                      
                                  }
                                """;
        var lexer = new Lexer(function);
        var cachedEnumerable = new CachedEnumerable<Token>(lexer.Lex(true));
        Assert.True(FunctionNode.IsFunctionNode(out _, cachedEnumerable));
    }
}