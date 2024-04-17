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
        bool NextEq(Func<char, bool> predicate)
        {
            return _index < source.Length && predicate(source[_index]);
        }
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
                case ".":
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
                        while (!outOfBounds && source[_index] != '\n')
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
                yield return new Token(new Identifier(token), _line, _column);
                continue;
            }
            #endregion
        }
        yield return new Token(new EndOfFile(), _line, _column);
    }

}
