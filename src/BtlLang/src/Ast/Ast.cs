using BtlLang.Tokens;
using System.Collections.Generic;

namespace BtlLang.Ast;

public abstract class ParseException(string message) : Exception(message);

public class UnexpectedTokenException(Token token): ParseException($"Unexpected token: {token.Type} at line {token.Line}, column {token.Column}");
public class KeywordException(Token token) : ParseException($"Unexpected keyword: {((Keyword)token.Type).Value} at line {token.Line}, column {token.Column}");
public class PunctuationException(Token token) : ParseException($"Unexpected token: {token.Type} at line {token.Line}, column {token.Column}");
public class IdentifierException(Token token): ParseException($"Unexpected identifier: {((Identifier)token.Type).Value} at line {token.Line}, column {token.Column}");
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
    public Queue<Node> Nodes { get; internal set; } = [];
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
                        Nodes.Enqueue(new FunctionNode(tokens));
                        break;
                    case "struct":
                        Nodes.Enqueue(new StructNode(tokens));
                        break;
                    case "const":
                    case "var":
                        Nodes.Enqueue(new VariableNode(tokens));
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
                        Nodes.Enqueue(new BlockNode(tokens));
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
    public ParameterNode(IEnumerator<Token> tokens)
    {
        while (tokens.MoveNext())
        {
            if (Name is not null && Type is not null)
            {
                break;
            }
            if (tokens.Current.Type.IsWhitespace() || tokens.Current.Type.IsComment())
            {
                continue;
            }
            switch (tokens.Current.Type)
            {
                case Identifier id:
                    
                    if (Type is null)
                    {
                        Type = id;
                    }
                    else if (Name is null)
                    {
                        Name = id;
                    }
                    continue;
                case Punctuation punc:
                    switch (punc.Value)
                    {
                        case ",":
                            continue;
                        default:
                            throw new PunctuationException(tokens.Current);
                    }
                default:
                    throw new UnexpectedTokenException(tokens.Current);
            }
        }
    }
}

/**
Represents a function node in the AST
Syntax is as follows:

```txt
 func $ident ($parameters*): $ident? {
    $block
 }
```
*/
public class FunctionNode : Node
{
    public Identifier Name { get; private set; }
    public Queue<ParameterNode> Parameters { get; private set; } = [];
    public Identifier ReturnType { get; private set; }
    public BlockNode Body { get; private set; }
    public FunctionNode(IEnumerator<Token> tokens)
    {
        if(tokens.Current.Type.TryParse<Keyword>(new KeywordException(tokens.Current)).Value != "func")
        {
            throw new KeywordException(tokens.Current);
        }
        
        var parsingParameters = false;
        var parsingReturnType = false;

        while (tokens.MoveNext())
        {
            // base case. We have finished parsing this function.
            // Parameters are optional, but the name and body are required.
            if (Body is not null && Name is not null && ReturnType is not null)
            {
                break;
            }
            
            
            var current = tokens.Current;
            if (current.Type.IsWhitespace() || current.Type.IsComment())
            {
                continue;
            }
            
            if (parsingReturnType)
            {
                ReturnType = current.Type.TryParse<Identifier>(new IdentifierException(current));
                parsingReturnType = false;
                continue;
            }

            if (parsingParameters)
            {
                // check for end of parameters
                if (current.Type is Punctuation { Value: ")" })
                {
                    parsingParameters = false;
                    continue;
                }
                Parameters.Enqueue(new ParameterNode(tokens));
            }

            switch (current.Type)
            {
                case Identifier id when Name is null:
                {
                    Name = id;
                    continue;
                }
                case Punctuation { Value: "(" }:
                {
                    if (Parameters.Count > 0)
                    {
                        throw new PunctuationException(tokens.Current);
                    }
                    parsingParameters = true;
                    Parameters.Enqueue(new ParameterNode(tokens));
                    continue;
                }
                case Punctuation { Value: ":" }:
                {
                    parsingReturnType = true;
                    continue;
                }
                case Punctuation { Value: "{" } when Body is null && Name is not null:
                {
                    // if we have not parsed a return type, default to void
                    ReturnType = new Identifier("void");
                    Body = new BlockNode(tokens);
                    break;
                }
                default:
                    throw new UnexpectedTokenException(current);
            }
        }
    }
}

public class BlockNode : Node
{
    public BlockNode(IEnumerator<Token> tokens)
    {
        while (tokens.MoveNext())
        {
            var current = tokens.Current;
            if (current.Type.IsWhitespace() || current.Type.IsComment())
            {
                continue;
            }
            if (current.Type is Punctuation { Value: "}" })
            {
                break;
            }


            switch (tokens.Current.Type)
            {
                case Keyword keyword:
                {
                    switch (keyword.Value)
                    {
                        
                    }

                    continue;
                }
            }
        }
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
    public Token Mutability { get; private set; }
    public Identifier Name { get; private set; }
    public Identifier Type { get; private set; }
    public ExpressionNode Value { get; private set; }
    public VariableNode(IEnumerator<Token> tokens)
    {
        
    }
}

#region Expressions

public class ExpressionNode : Node;

public class BinaryExpression : ExpressionNode
{
    public Operator Operator { get; private set; }
    public ExpressionNode Left { get; private set; }
    public ExpressionNode Right { get; private set; }
    public BinaryExpression(IEnumerator<Token> tokens)
    {
        
    }
}

public class AssignmentExpression : Node
{
    public Token Value { get; private set; }
    public AssignmentExpression(IEnumerator<Token> tokens)
    {
        
    }
}

#endregion

#endregion