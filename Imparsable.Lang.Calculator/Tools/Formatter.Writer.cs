using System.Text;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private sealed class Writer(SyntaxTree tree)
    {
        private const int TabSize = 4;
        private const int MaxBlankLines = 2;
        private const int MaxNewLines = MaxBlankLines + 1;

        private readonly TriviaStream _trivia = new(tree.Trivia);
        private readonly StringBuilder _builder = new(tree.Source.Text.Length);

        private bool _pendingSpace;
        private bool _pendingIndent;
        private int _requiredNewLines;
        private int _writtenNewLines;

        public int Depth { get; set; }

        public void Space() => _pendingSpace = true;
        public void Indent() => _pendingIndent = true;
        public void NewLine() => _requiredNewLines = Math.Max(_requiredNewLines, 1);

        public void Write(Lexer<Token>.Token token)
        {
            WriteTrivia(token);
            FlushLayout();

            _builder.Append(tree.Source.GetText(token.Offset, token.Length));

            _writtenNewLines = 0;
            _pendingIndent = false;
        }

        public void WriteLine(Lexer<Token>.Token token)
        {
            Write(token);
            _requiredNewLines = Math.Max(_requiredNewLines, 1);
        }

        private void FlushLayout()
        {
            if (_requiredNewLines > 0)
            {
                _pendingSpace = false;
                EnsureNewLines(_requiredNewLines);
                _requiredNewLines = 0;
            }

            if (_writtenNewLines > 0 || _builder.Length == 0)
            {
                _pendingSpace = false;

                if (_pendingIndent)
                    _builder.Append(' ', Depth * TabSize);
            }
            else if (_pendingSpace)
            {
                _builder.Append(' ');
            }

            _pendingSpace = false;
        }

        private void EnsureNewLines(int count)
        {
            _pendingSpace = false;

            count = Math.Min(count, MaxNewLines);

            while (_writtenNewLines < count)
            {
                _builder.Append(Environment.NewLine);
                _writtenNewLines++;
            }
        }

        private void WriteTrivia(Lexer<Token>.Token token)
        {
            foreach (var trivia in _trivia.Read(token))
            {
                if (trivia is null) continue;

                switch (trivia.Value.Type)
                {
                    case Token.COMMENT:
                    {
                        WriteComment(trivia.Value);
                        break;
                    }

                    case Token.NEWLINE:
                    {
                        var count = trivia.Value.Length / Environment.NewLine.Length;
                    
                        EnsureNewLines(Math.Max(_requiredNewLines, count));
                    
                        _requiredNewLines = 0;
                    
                        break;
                    }
                    
                    case Token.WHITESPACE:
                    {
                        Space();
                        break;
                    }
                }
            }
        }

        private void WriteComment(Lexer<Token>.Token token)
        {
            var text = tree.Source.GetText(token.Offset, token.Length);

            if (_writtenNewLines > 0 || _builder.Length == 0)
            {
                _pendingSpace = false;

                if (_pendingIndent)
                    _builder.Append(' ', Depth * TabSize);
            }
            else if (_pendingSpace)
            {
                _builder.Append(' ');
                _pendingSpace = false;
            }

            switch (text)
            {
                case ['/', '/', ..]:
                {
                    _builder.Append(text);
                    _writtenNewLines = 0;

                    EnsureNewLines(Math.Max(_requiredNewLines, 1));

                    _requiredNewLines = 0;

                    break;
                }

                case ['/', '*', ..]:
                {
                    _builder.Append(text);
                    _writtenNewLines = 0;
                    break;
                }
            }
        }

        public string Finish()
        {
            _pendingSpace = false;

            EnsureNewLines(_requiredNewLines);

            _requiredNewLines = 0;

            return _builder.ToString();
        }
    }
}