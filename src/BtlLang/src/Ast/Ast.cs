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
    public Identifier Name { get; set; }
    public Identifier Type { get; set; } 
    
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
    public Identifier Name { get; internal set; }
    public Queue<ParameterNode> Parameters { get; internal set; } = [];
    public Identifier ReturnType { get; internal set; }
    public BlockNode Body { get; internal set; }

    public static bool IsFunctionNode(out FunctionNode node, CachedEnumerable<Token> tokens)
    {
        node = default;
        if (tokens.Current.Type is not Keyword { Value: "func" })
        {
            return false;
        }
        
        Identifier name = null;
        Queue<ParameterNode> parameters = [];
        Identifier returnType = null;
        BlockNode body = null;

        while (tokens.MoveNext())
        {
            if (name is null)
            {
                name = tokens.Current.Type.TryParse<Identifier>(new UnexpectedTokenException(tokens.Current));
                continue;
            }

            // we have not yet passed params
            if (returnType is null && ParameterNode.IsParameterNode(out var parameterNode, tokens, typeof(FunctionNode)))
            {
                parameters.Enqueue(parameterNode);
                continue;
            }
        }

        // TODO: Implement the rest of the function node
        return false;
    }
}

public class BlockNode : Node
{
    
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