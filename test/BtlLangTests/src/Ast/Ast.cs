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
                                      
                                      func add(a: int, b: int): int
                                      {
                                          return a + b;
                                      }
                                   """;


    [Fact]
    public void TestParameterParsing()
    {
        var lexer = new Lexer("a: int, b: int)");
        var tokens = lexer.Lex(true);
        using var enumerator = tokens.GetEnumerator();
        var punc = new ParameterNode(enumerator, new Punctuation(","), new Punctuation(")"));
        Assert.Equal("a", punc.Name.Value);
        Assert.Equal("int", punc.Type.Value);
        punc = new ParameterNode(enumerator, new Punctuation(","), new Punctuation(")"));
        Assert.Equal("b", punc.Name.Value);
        Assert.Equal("int", punc.Type.Value);
    }
}