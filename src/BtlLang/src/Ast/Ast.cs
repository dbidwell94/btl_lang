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
        Root = new RootNode(new CachedEnumerable<Token>(tokens));
    }
}

#region Nodes
    
public abstract class Node
{
    public Queue<Node> Nodes { get; internal set; } = [];
}

public class RootNode : Node
{
    public RootNode(CachedEnumerable<Token> tokens)
    {
        while (tokens.MoveNext())
        {
            if (FunctionNode.IsFunctionNode(out var node, tokens))
            {
                Nodes.Enqueue(node);
                continue;
            }
            if (StructNode.IsStructNode(out var structNode, tokens))
            {
                Nodes.Enqueue(structNode);
                continue;
            }
        }
    }
}
public class ParameterNode : Node
{
    public Identifier Name { get; private set; }
    public Identifier Type { get; private set; } 
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="tokens"></param>
    /// <param name="parentType">What type is the parent node? Expected func or struct, otherwise false</param>
    /// <returns></returns>
    public static bool IsParameterNode(out ParameterNode node, CachedEnumerable<Token> tokens, Type parentType)
    {
        node = default!;
        var toReturn = false;
        int movedAmount = 0;
        
        if (parentType != typeof(FunctionNode) && parentType != typeof(StructNode))
        {
            return toReturn;
        }

        while (tokens.MoveNext())
        {
            if (tokens.Current.Type.IsWhitespace() || tokens.Current.Type.IsComment())
            {
                continue;
            }
            movedAmount++;
            var current = tokens.Current;
            if (current.Type.IsIdentifier())
            {
                node = new ParameterNode
                {
                    Name = (Identifier)current.Type
                };
            }
            else if (current.Type.IsPunctuation())
            {
                if (((Punctuation)current.Type).Value == ":")
                {
                    if (node is null)
                    {
                        break;
                    }
                    if (!tokens.MoveNext())
                    {
                        break;
                    }
                    movedAmount++;
                    current = tokens.Current;
                    if (current.Type.IsIdentifier())
                    {
                        node.Type = (Identifier)current.Type;
                        toReturn = true;
                    }

                    break;
                }
            }
        }

        if (!toReturn)
        {
            node = null;
            tokens.Seek(-movedAmount);
            return toReturn;
        }

        tokens.MoveNext();

        // If parentType is FunctionNode and the next token is not a comma or closing parenthesis, throw an exception
        if (parentType == typeof(FunctionNode) && !tokens.Current.Type.IsPunctuation())
        {
            throw new PunctuationException(tokens.Current);
        }
        if (((Punctuation)tokens.Current.Type).Value != "," && ((Punctuation)tokens.Current.Type).Value != ")")
        {
            throw new PunctuationException(tokens.Current);
        }
        
        return toReturn;
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

    public static bool IsFunctionNode(out FunctionNode node, CachedEnumerable<Token> tokens)
    {
        node = default;
        var foundFunctionKeyword = false;
        
        Identifier name = null;
        Queue<ParameterNode> parameters = [];
        Identifier returnType = null;
        BlockNode body = null;

        while (tokens.MoveNext())
        {
            if (tokens.Current.Type.IsWhitespace() || tokens.Current.Type.IsComment())
            {
                continue;
            }

            if (name is null && !foundFunctionKeyword && tokens.Current.Type is Keyword { Value: "func" })
            {
                foundFunctionKeyword = true;
                continue;
            }
            if(!foundFunctionKeyword && name is null && tokens.Current.Type is not Keyword {Value: "func"})
            {
                return false;
            }
            
            
            // we have not yet parsed the name
            if (name is null)
            {
                name = tokens.Current.Type.TryParse<Identifier>(new UnexpectedTokenException(tokens.Current));
                continue;
            }
            
            // we have not yet parsed the return type
            if (returnType is null && tokens.Current.Type.IsPunctuation())
            {
                switch (tokens.Current.Type)
                {
                    case Punctuation { Value: ":" } when !tokens.MoveNext():
                        throw new UnexpectedEndOfFileException(tokens.Current);
                    case Punctuation { Value: ":" }:
                        returnType = tokens.Current.Type.TryParse<Identifier>(new UnexpectedTokenException(tokens.Current));
                        continue;
                    case Punctuation { Value: "{" }:
                        returnType = new Identifier("void");
                        break;
                }
            }
            
            // we have not yet parsed params
            if (returnType is null)
            {
                while (ParameterNode.IsParameterNode(out var parameterNode, tokens, typeof(FunctionNode)))
                {
                    parameters.Enqueue(parameterNode);
                }
                continue;
            }

            // we have not yet parsed the body
            if (body is null && BlockNode.IsBlockNode(out var blockNode, tokens))
            {
                body = blockNode;
                break;
            }
        }

        if (name is null || returnType is null || body is null)
        {
            throw new UnexpectedEndOfFileException(tokens.Current);
        }

        node = new FunctionNode
        {
            Name = name,
            Body = body,
            Parameters = parameters,
            ReturnType = returnType
        };
        return true;
    }
}

public class BlockNode : Node
{
    public static bool IsBlockNode(out BlockNode node, CachedEnumerable<Token> tokens)
    {
        node = null;
        
        var foundOpeningBrace = tokens.Current.Type is Punctuation { Value: "{" };
        var foundClosingBrace = false;

        Queue<Node> nodes = [];

        while (tokens.MoveNext() && !foundClosingBrace)
        {
            if (tokens.Current.Type.IsWhitespace() || tokens.Current.Type.IsComment())
            {
                continue;
            }
            
            switch (foundOpeningBrace)
            {
                case false when tokens.Current.Type is Punctuation { Value: "{" }:
                    foundOpeningBrace = true;
                    continue;
                case false when tokens.Current.Type is not Punctuation { Value: "{" }:
                    return false;
            }

            foundClosingBrace = foundClosingBrace switch
            {
                false when tokens.Current.Type is Punctuation { Value: "}" } => true,
                _ => foundClosingBrace
            };
            if (foundClosingBrace)
            {
                break;
            }

            if (FunctionNode.IsFunctionNode(out var functionNode, tokens))
            {
                nodes.Enqueue(functionNode);
                continue;
            }

            if (StructNode.IsStructNode(out var structNode, tokens))
            {
                nodes.Enqueue(structNode);
                continue;
            }

            if (IsBlockNode(out var blockNode, tokens))
            {
                nodes.Enqueue(blockNode);
            }
        }
        
        if (!foundOpeningBrace || !foundClosingBrace)
        {
            throw new UnexpectedEndOfFileException(tokens.Current);
        }

        node = new BlockNode
        {
            Nodes = nodes
        };
        return true;
    }
}

public class StructNode : Node
{
    public Identifier Name { get; private set; }
    public List<ParameterNode> Fields { get; private set; }
    
    public static bool IsStructNode(out StructNode node, CachedEnumerable<Token> tokens)
    {
        node = null;
        return false;
        // TODO: Implement the struct node
    }
}

public class VariableNode : Node
{
    public Token Mutability { get; private set; }
    public Identifier Name { get; private set; }
    public Identifier Type { get; private set; }
    public ExpressionNode Value { get; private set; }
    public VariableNode(CachedEnumerable<Token> tokens)
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