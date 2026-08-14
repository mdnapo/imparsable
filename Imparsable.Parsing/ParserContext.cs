using Imparsable.Parsing.Exceptions;

namespace Imparsable.Parsing;

public class ParserContext<TToken>(ParserConfiguration<TToken> configuration, List<Lexer<TToken>.Token> tokens)
    : Stream<Lexer<TToken>.Token>(tokens.ToArray()) where TToken : Enum
{
    public bool CheckOffset(int offset, TToken type)
    {
        if (Ended()) return false;
        if (Equals(Peek(offset).Type, configuration.End)) return false;
        return Equals(Peek(offset).Type, type);
    }

    public bool Check(TToken type)
    {
        if (Ended()) return false;
        if (Equals(Peek().Type, configuration.End)) return false;
        return Equals(Peek().Type, type);
    }

    public bool CheckSequence(params TToken[] sequence)
    {
        for (var i = 0; i < sequence.Length; i++)
        {
            if (!CheckOffset(i, sequence[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckAny(params TToken[] options)
    {
        for (var i = 0; i < options.Length; i++)
        {
            if (Check(options[i]))
            {
                return true;
            }
        }

        return false;
    }

    public override bool Ended() => base.Ended() || Equals(Current.Type, configuration.End);

    public bool Match(TToken type)
    {
        if (!Check(type)) return false;
        Advance();
        return true;
    }

    public bool MatchAny(params TToken[] options)
    {
        for (var i = 0; i < options.Length; i++)
        {
            if (Check(options[i]))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    public bool MatchSequence(params TToken[] sequence)
    {
        for (var i = 0; i < sequence.Length; i++)
        {
            if (!CheckOffset(i, sequence[i]))
            {
                return false;
            }
        }

        for (var i = 0; i < sequence.Length; i++)
        {
            Advance();
        }

        return true;
    }

    public Lexer<TToken>.Token Previous()
    {
        return Peek(-1);
    }

    public Lexer<TToken>.Token ConsumeAny(TToken[] options, string message)
    {
        foreach (var option in options)
        {
            if (Check(option))
            {
                return Advance();
            }
        }

        throw Halt(message);
    }

    public Lexer<TToken>.Token Consume(TToken type, string message)
    {
        if (Check(type)) return Advance();
        throw Halt(message);
    }

    public SyntaxException Halt(string message) => throw new SyntaxException(Current, message);
}