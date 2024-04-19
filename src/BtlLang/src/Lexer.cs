using BtlLang.Tokens;

namespace BtlLang;

public class LexException(string message, int column, int line) : Exception(message)
{
    public int Column = column;
    public int Line = line;
}

public class Lexer(string source)
{
    private int _index;
    private int _line = 1;
    private int _column;

    /// <summary>
    /// Lexes the source code and returns a sequence of tokens.
    /// </summary>
    /// <param name="ignoreWhitespace">Whether to ignore whitespace tokens.</param>
    /// <exception cref="LexException">Thrown when an error occurs during lexing.</exception>
    public IEnumerable<Token> Lex(bool ignoreWhitespace = false)
    {
        var buffer = "";
        while (_index < source.Length)
        {
            var character = source[_index];
            _index++;
            var outOfBounds = _index >= source.Length;
            buffer += character;
            _column++;
            switch (buffer)
            {
                #region Whitespace and Newline
                case " ":
                case "\t":
                    {
                        buffer = "";
                        if (!ignoreWhitespace)
                        {
                            yield return new Token(new Whitespace(), _line, _column);
                        }
                        continue;
                    }
                case "\r" when NextEq(c => c == '\n'):
                {
                    buffer = "";
                    if (!ignoreWhitespace)
                    {
                        yield return new Token(new Whitespace(), _line, _column);
                    }
                    _line++;
                    _column = 1;
                    _index++;
                    continue;
                }
                case "\n":
                    {
                        buffer = "";
                        if (!ignoreWhitespace)
                        {
                            yield return new Token(new Whitespace(), _line, _column);
                        }
                        _line++;
                        _column = 1;
                        continue;

                    }
                #endregion

                #region Dual Character Operators
                case "=" when NextEq(c => c == '='):
                case "!" when NextEq(c => c == '='):
                case "<" when NextEq(c => c == '='):
                case ">" when NextEq(c => c == '='):
                case "*" when NextEq(c => c == '='):
                case "/" when NextEq(c => c == '='):
                case "&" when NextEq(c => c == '&'):
                case "|" when NextEq(c => c == '|'):
                case ">" when NextEq(c => c == '>'):
                case "<" when NextEq(c => c == '<'):
                case "+" when NextEq(c => c == '+'):
                case "-" when NextEq(c => c == '-'):
                    {
                        buffer += source[_index];
                        _index++;
                        _column++;
                        var token = buffer;
                        buffer = "";
                        yield return new Token(new Operator(token), _line, _column);
                        continue;
                    }
                #endregion

                #region Single Character Operators
                // only divide (/), not comment (//)
                case "/" when NextEq(c => c != '/'):
                case "=" when NextEq(c => c != '='):
                case "&" when NextEq(c => c != '&'):
                case "|" when NextEq(c => c != '|'):
                case "!":
                case "+":
                case "-":
                case "*":
                case "%":
                case "~":
                case "^":
                case "<":
                case ">":
                    {
                        var token = buffer;
                        buffer = "";
                        yield return new Token(new Operator(token), _line, _column);
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
                case "." when !NextEq(char.IsDigit):
                case ";":
                    {
                        var token = buffer;
                        buffer = "";
                        yield return new Token(new Punctuation(token), _line, _column);
                        continue;
                    }
                #endregion

                #region Comments
                case "/" when NextEq(c => c == '/'):
                    {
                        while (!outOfBounds && source[_index] != '\n' && source[_index] != '\r')
                        {
                            buffer += source[_index];
                            _index++;
                            _column++;
                            outOfBounds = _index >= source.Length;
                        }
                        var token = buffer;
                        buffer = "";
                        yield return new Token(new Comment(token), _line, _column);
                        buffer = "";
                        continue;
                    }
                #endregion

                #region Literal String or Character
                case "\"" or "'":
                    {
                        var quote = buffer;
                        while (!outOfBounds && source[_index].ToString() != quote)
                        {
                            buffer += source[_index];
                            _index++;
                            _column++;
                            outOfBounds = _index >= source.Length;
                        }
                        _index++;
                        _column++;
                        buffer += quote;
                        var token = buffer;
                        buffer = "";
                        yield return new Token(new Literal(token), _line, _column);
                        buffer = "";
                        continue;
                    }
                    #endregion
            }

            #region Identifier or Keyword
            if (char.IsLetter(character) || character == '_')
            {
                while (!outOfBounds && (char.IsLetterOrDigit(source[_index]) || source[_index] == '_'))
                {
                    buffer += source[_index];
                    _index++;
                    _column++;
                    outOfBounds = _index >= source.Length;
                }
                var token = buffer;
                buffer = "";
                yield return ParseKeywordOrIdentifier(token);
                continue;
            }
            #endregion
            
            #region Number
            if(char.IsDigit(character) || character == '.')
            {
                var alreadyUsedDot = character == '.';
                while (!outOfBounds && (char.IsDigit(source[_index]) || (!alreadyUsedDot && source[_index] == '.')))
                {
                    if (source[_index] == '.')
                    {
                        alreadyUsedDot = true;
                    }
                    buffer += source[_index];
                    _index++;
                    _column++;
                    outOfBounds = _index >= source.Length;
                }
                var token = buffer;
                buffer = "";
                yield return new Token(new Literal(token), _line, _column);
                continue;
            }
            #endregion
        }
        yield return new Token(new EndOfFile(), _line, _column);
        yield break;

        bool NextEq(Func<char, bool> predicate)
        {
            return _index < source.Length && predicate(source[_index]);
        }
    }
    
    private Token ParseKeywordOrIdentifier(string buffer)
    {
        return buffer switch
        {
            Keywords.If => new Token(new Keyword(Keywords.If), _line, _column),
            Keywords.Else => new Token(new Keyword(Keywords.Else), _line, _column),
            Keywords.While => new Token(new Keyword(Keywords.While), _line, _column),
            Keywords.For => new Token(new Keyword(Keywords.For), _line, _column),
            Keywords.Return => new Token(new Keyword(Keywords.Return), _line, _column),
            Keywords.Break => new Token(new Keyword(Keywords.Break), _line, _column),
            Keywords.Continue => new Token(new Keyword(Keywords.Continue), _line, _column),
            Keywords.True => new Token(new Keyword(Keywords.True), _line, _column),
            Keywords.False => new Token(new Keyword(Keywords.False), _line, _column),
            Keywords.Null => new Token(new Keyword(Keywords.Null), _line, _column),
            Keywords.Func => new Token(new Keyword(Keywords.Func), _line, _column),
            Keywords.Struct => new Token(new Keyword(Keywords.Struct), _line, _column),
            Keywords.Loop => new Token(new Keyword(Keywords.Loop), _line, _column),
            Keywords.Var => new Token(new Keyword(Keywords.Var), _line, _column),
            Keywords.Const => new Token(new Keyword(Keywords.Const), _line, _column),
            _ => new Token(new Identifier(buffer), _line, _column)
        };
    }

}
