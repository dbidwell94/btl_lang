using BtlLang;
using BtlLang.Ast;
using BtlLang.Tokens;

namespace BtlLangTests.Ast;

public class AstTest
{
    private const string Source = """
                                      const globalVar: string = "Hello, World!";
                                   
                                      // Entrypoint of the program
                                      func main()
                                      {
                                          var localVar: int = add(1, 4);
                                      }
                                      
                                      func add(int a, int b): int
                                      {
                                          return a + b;
                                      }
                                   """;

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