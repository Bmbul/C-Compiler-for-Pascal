using Common;

namespace Parser.Parser.ParserStateMachine
{
    public interface IParsingState
    {
        IParsingState Handle(LexicalToken token);
    }
}