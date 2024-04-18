using BtlLang.Tokens;

namespace BtlLang.Ast;

public abstract class ParseException(string message) : Exception(message);

public class KeywordException(Token token) : ParseException($"Unexpected keyword: {((Keyword)token.Type).Value} at line {token.Line}, column {token.Column}");
public class PunctuationException(Token token) : ParseException($"Unexpected token: {token.Type} at line {token.Line}, column {token.Column}");
public class UnexpectedEndOfFileException(Token token) : ParseException($"Unexpected end of file at line {token.Line}, column {token.Column}");

public class Ast
{
    public RootNode Root { get; private set; }
    public Ast(Lexer lexer)
    {
        var tokens = lexer.Lex(true);
        using var enumerator = tokens.GetEnumerator();
        Root = new RootNode(enumerator);
    }
}

#region Nodes
    
public abstract class Node
{
    public List<Node> Nodes { get; internal set; } = [];
}

public class RootNode : Node
{
    public RootNode(IEnumerator<Token> tokens)
    {
        while (tokens.MoveNext())
        {
            var current = tokens.Current;
            if (current.Type.IsKeyword())
            {
                switch (((Keyword)current.Type).Value)
                {
                    case "func":
                        Nodes.Add(new FunctionNode(tokens));
                        break;
                    case "struct":
                        Nodes.Add(new StructNode(tokens));
                        break;
                    case "const":
                    case "var":
                        Nodes.Add(new VariableNode(tokens));
                        break;
                    default:
                        throw new KeywordException(current);
                }
            }
            else if (current.Type.IsPunctuation())
            {
                switch (((Punctuation)current.Type).Value)
                {
                    case "{":
                        Nodes.Add(new BlockNode(tokens));
                        break;
                    default:
                        throw new PunctuationException(current);
                }
            }
        }
    }
}
public class ParameterNode : Node
{
    public Identifier Name { get; }
    public Identifier Type { get; } 
    public ParameterNode(IEnumerator<Token> tokens, Punctuation delimiter, Punctuation start)
    {
        while (tokens.MoveNext())
        {
            if (tokens.Current.Type.IsWhitespace() || tokens.Current.Type.IsComment())
            {
                continue;
            }

            switch (tokens.Current.Type)
            {
                case Identifier id:
                    if (Name is null)
                    {
                        Name = id;
                    }
                    else if (this.Type is null)
                    {
                        Type = id;
                    }
                    else
                    {
                        throw new KeywordException(tokens.Current);
                    }
                    break;
                case Punctuation colon when colon.Value == ":":
                    continue;
                case Punctuation punc when punc.Value == delimiter.Value || punc.Value == start.Value:
                    return;
                default:
                    throw new PunctuationException(tokens.Current);
            }
        }
    }
}

public class FunctionNode : Node
{
    public Identifier Name { get; private set; }
    public List<ParameterNode> Parameters { get; private set; }
    public Identifier ReturnType { get; private set; }
    public BlockNode Body { get; private set; }
    public FunctionNode(IEnumerator<Token> tokens)
    {
        
    }
}

public class BlockNode : Node
{
    public BlockNode(IEnumerator<Token> tokens)
    {
        
    }
}

public class StructNode : Node
{
    public Identifier Name { get; private set; }
    public List<ParameterNode> Fields { get; private set; }
    public StructNode(IEnumerator<Token> tokens)
    {
        
    }
}

public class VariableNode : Node
{
    public Identifier Name { get; private set; }
    public Identifier Type { get; private set; }
    public Token Value { get; private set; }
    public VariableNode(IEnumerator<Token> tokens)
    {
        
    }
}

#endregion