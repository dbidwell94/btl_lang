using BtlLang.Tokens;

namespace BtlLang;

public class Ast(Lexer lexer)
{
    // This represents the root of the AST. This will contain all the global variables and functions.
    private TreeNode _global;
    public void BuildTree()
    {
        foreach (var token in lexer.Lex(true))
        {
            
        }
    }
}

public struct TreeNode(Token token)
{
    public readonly Token Token = token;
    public List<TreeNode> Children = new();
}