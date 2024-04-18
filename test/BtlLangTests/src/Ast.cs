using BtlLang;

namespace BtlLangTests;

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
    public void TestBuildTree()
    {
        var lexer = new Lexer(Source);
        var ast = new Ast(lexer);
        ast.BuildTree();
    }
}