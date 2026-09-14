using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private sealed class Document(SyntaxTree tree) : IEnumerable<Chunk>
    {
        private readonly List<Chunk> _chunks = [];

        private Split _split;
        private int _depth;

        public void IncrementDepth() => ++_depth;
        public void DecrementDepth() => --_depth;

        public void Space()
        {
            if (_split == Split.NONE)
                _split = Split.SPACE;
        }

        public void NewLine()
        {
            if (_split < Split.LINE)
                _split = Split.LINE;
        }

        public void Write(Lexer<Token>.Token token)
        {
            WriteTrivia(token);

            Add(tree.Source.GetText(token.Offset, token.Length));
        }

        public void WriteLine(Lexer<Token>.Token token)
        {
            Write(token);
            NewLine();
        }

        private void ApplyCommentSplit(Split split)
        {
            if (_chunks.Count > 0 && split < Split.LINE)
            {
                _split = Split.SPACE;
                return;
            }

            ApplyTriviaSplit(split);
        }

        private void WriteTrivia(Lexer<Token>.Token token)
        {
            if (tree.GetTrivia(token) is not { Length: > 0 } trivia)
                return;

            var split = Split.NONE;

            foreach (var value in trivia)
            {
                switch (value.Type)
                {
                    case Token.COMMENT:
                    {
                        ApplyCommentSplit(split);
                        split = Split.NONE;

                        WriteComment(value);
                        break;
                    }

                    case Token.NEWLINE:
                    {
                        var count = value.Length / Environment.NewLine.Length;

                        split = AddNewLines(split, count);
                        break;
                    }

                    case Token.WHITESPACE:
                    {
                        if (split == Split.NONE)
                            split = Split.SPACE;

                        break;
                    }
                }
            }

            ApplyTriviaSplit(split);
        }

        private void WriteComment(Lexer<Token>.Token token)
        {
            var text = tree.Source.GetText(token.Offset, token.Length);

            Add(text);

            if (text is ['/', '/', ..])
                NewLine();
        }

        private void Add(string text)
        {
            _chunks.Add(new Chunk(
                Text: text,
                SplitBefore: _split,
                Depth: _depth
            ));

            _split = Split.NONE;
        }

        private void ApplyTriviaSplit(Split split)
        {
            if (split == Split.NONE)
                return;

            _split = _split switch
            {
                Split.NONE => split,
                Split.SPACE => Split.SPACE,
                _ => Max(_split, split)
            };
        }

        private static Split AddNewLines(Split split, int count)
        {
            while (count-- > 0)
            {
                split = split switch
                {
                    Split.NONE or Split.SPACE => Split.LINE,
                    Split.LINE => Split.BLANK_LINE,
                    _ => Split.TWO_BLANK_LINES
                };
            }

            return split;
        }

        private static Split Max(Split x, Split y) => x >= y ? x : y;

        public IEnumerator<Chunk> GetEnumerator() => _chunks.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}