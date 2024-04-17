namespace BtlLang;

public class LexException(string message, int column, int line) : Exception(message)
{
    public int Column = column;
    public int Line = line;
}

public enum TokenType
{
    Identifier,
    Type,
    Operator,
    Literal,
    Keyword,
    Punctuation,
    Comment,
    Whitespace,
    Newline,
    EndOfFile
}

public struct Token(TokenType type, string value, int line, int column)
{
    public TokenType Type = type;
    public string Value = value;
    public int Line = line;
    public int Column = column;
}

public class Lexer(string source)
{
    private readonly string source = source;

    private int index = 0;
    private int line = 1;
    private int column = 0;

    /// <summary>
    /// Lexes the source code and returns a sequence of tokens.
    /// </summary>
    /// <param name="ignoreWhitespace">Whether to ignore whitespace tokens.</param>
    /// <exception cref="LexException">Thrown when an error occurs during lexing.</exception>
    public IEnumerable<Token> Lex(bool ignoreWhitespace = false)
    {
        var buffer = "";
        while (index < source.Length)
        {
            var character = source[index];
            index++;
            var outOfBounds = index >= source.Length;
            buffer += character;
            column++;
            switch (buffer)
            {
                #region Whitespace and Newline
                case " ":
                case "\t":
                    {
                        var token = buffer;
                        buffer = "";
                        if (!ignoreWhitespace)
                        {
                            yield return new Token(TokenType.Whitespace, token, line, column);
                        }
                        continue;
                    }
                case "\n":
                    {
                        var token = buffer;
                        buffer = "";
                        if (!ignoreWhitespace)
                        {
                            yield return new Token(TokenType.Newline, token, line, column);
                        }
                        line++;
                        column = 1;
                        continue;

                    }
                #endregion

                #region Dual Character Operators
                case "=" when !outOfBounds && source[index] == '=':
                case "!" when !outOfBounds && source[index] == '=':
                case "<" when !outOfBounds && source[index] == '=':
                case ">" when !outOfBounds && source[index] == '=':
                case "*" when !outOfBounds && source[index] == '=':
                case "/" when !outOfBounds && source[index] == '=':
                case "&" when !outOfBounds && source[index] == '&':
                case "|" when !outOfBounds && source[index] == '|':
                case ">" when !outOfBounds && source[index] == '>':
                case "<" when !outOfBounds && source[index] == '<':
                case "+" when !outOfBounds && source[index] == '+':
                case "-" when !outOfBounds && source[index] == '-':

                    {
                        buffer += source[index];
                        index++;
                        column++;
                        var token = buffer;
                        buffer = "";
                        yield return new Token(TokenType.Operator, token, line, column);
                        continue;
                    }
                #endregion

                #region Single Character Operators
                case "+":
                case "-":
                case "*":
                // only divide (/), not comment (//)
                case "/" when !outOfBounds && source[index] != '/':
                case "%":
                case "=" when !outOfBounds && source[index] != '=':
                case "!" when !outOfBounds && source[index] != '=':
                case "&" when !outOfBounds && source[index] != '&':
                case "|" when !outOfBounds && source[index] != '|':
                case "~":
                case "^":
                case "<":
                case ">":
                    {
                        var token = buffer;
                        buffer = "";
                        yield return new Token(TokenType.Operator, token, line, column);
                        continue;
                    }
                #endregion

                #region Punctuation
                case "(":
                case ")":
                case "{":
                case "}":
                case "[":
                case "]":
                case ",":
                case ":":
                case ".":
                case ";":
                    {
                        var token = buffer;
                        buffer = "";
                        yield return new Token(TokenType.Punctuation, token, line, column);
                        continue;
                    }
                #endregion

                #region Comments
                case "/" when source[index] == '/':
                    {
                        while (!outOfBounds && source[index] != '\n')
                        {
                            buffer += source[index];
                            index++;
                            column++;
                            outOfBounds = index >= source.Length;
                        }
                        var token = buffer;
                        buffer = "";
                        yield return new Token(TokenType.Comment, token, line, column);
                        buffer = "";
                        continue;
                    }
                #endregion

                #region Literal String or Character
                case "\"" or "'":
                    {
                        var quote = buffer;
                        while (!outOfBounds && source[index].ToString() != quote)
                        {
                            buffer += source[index];
                            index++;
                            column++;
                            outOfBounds = index >= source.Length;
                        }
                        index++;
                        buffer += quote;
                        var token = buffer;
                        buffer = "";
                        yield return new Token(TokenType.Literal, token, line, column);
                        buffer = "";
                        continue;
                    }
                    #endregion
            }

            #region Identifier or Keyword
            if (char.IsLetter(character) || character == '_')
            {
                while (!outOfBounds && (char.IsLetterOrDigit(source[index]) || source[index] == '_'))
                {
                    buffer += source[index];
                    index++;
                    column++;
                    outOfBounds = index >= source.Length;
                }
                var token = buffer;
                buffer = "";
                yield return new Token(TokenType.Identifier, token, line, column);
                continue;
            }
            #endregion
        }
        yield return new Token(TokenType.EndOfFile, "", line, column);
    }

}
