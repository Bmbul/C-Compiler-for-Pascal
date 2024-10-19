using Common;
using Common.Utility;
using Parser.Parser.Exceptions;

namespace Parser.Parser.ParserStateMachine
{
    public class StartingState : ParsingStateBase
    {
        public StartingState(IParsingResultModifier parser) : base(parser) 
        {
        }
        
        public override IParsingState Handle(LexicalToken token)
        {
            if (token.IsKeyword(Constants.TypesToLexem[LexicalTokensEnum.ProgramKeyword]))
            {
                return new ProgramHeaderState(Parser);
            }
            throw new ParsingException("Expected program start.");
        }
    }
}