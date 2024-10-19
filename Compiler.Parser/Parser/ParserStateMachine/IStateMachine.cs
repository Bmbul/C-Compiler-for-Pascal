using Common;

namespace Parser.Parser.ParserStateMachine
{
    public interface IStateMachine
    {
        void ProcessTokens(LexicalToken token);
    }
}