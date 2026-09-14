using System.Text;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private class Document(SyntaxTree tree, int tabSize)
    {
        private readonly StringBuilder _builder = new(tree.Source.Text.Length);
        private readonly LinkedList<Lexer<Token>.Token> _entries = [];
        private int _depth;

        public void IncrementDepth() => ++_depth;
        public void DecrementDepth() => --_depth;
        public void Space() => _builder.Append(' ');
        public void Indent() => _builder.Append(' ', tabSize * _depth);
        public void NewLine() => _builder.Append(Environment.NewLine);

        public void Write(Lexer<Token>.Token token)
        {
            WriteTrivia(token);
            _builder.Append(tree.Source.GetText(token.Offset, token.Length));
        }

        public void WriteLine(Lexer<Token>.Token token)
        {
            Write(token);
            NewLine();
        }

        public void WriteTrivia(Lexer<Token>.Token token)
        {
            if (tree.GetTrivia(token) is not { Length: > 0 } trivia)
                return;

            _entries.Clear();

            foreach (var value in trivia)
            {
                switch (value.Type)
                {
                    case Token.COMMENT:
                    {
                        var text = tree.Source.GetText(value.Offset, value.Length);
                        _builder.Append(text);
                        _entries.AddLast(value);
                        break;
                    }

                    case Token.NEWLINE:
                    {
                        var previous = _builder[^1] == '\n' ? 1 : 0;
                        var node = _entries.Last;

                        while (node is { Value.Type: Token.NEWLINE })
                        {
                            previous++;
                            node = node.Previous;
                        }

                        if (previous < 2)
                        {
                            _builder.Append(Environment.NewLine);
                            _entries.AddLast(value);
                        }

                        break;
                    }

                    case Token.WHITESPACE:
                    {
                        if (token is { Type: Token.SEMICOLON })
                            break;

                        if (_builder[^1] == ' ')
                            break;

                        if (_entries.Last is { Value.Type: Token.NEWLINE })
                            break;

                        _builder.Append(' ');
                        _entries.AddLast(value);
                        break;
                    }
                }
            }

            _entries.Clear();
        }

        public override string ToString() => _builder.ToString();
    }
}