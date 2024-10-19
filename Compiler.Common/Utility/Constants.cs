using System.Collections.Generic;
namespace Common.Utility
{
    public static class Constants
    {
        public const int MaxLexemLength = 50;

        public static readonly List<string> Keywords = new List<string> { "program", "var", "begin", "end", "integer", "string" };
        public static readonly List<string> LexicalTokens = new List<string> { "+", "-", ":=", ":", ";", ".", "," };
        public static readonly List<char> EndingChars = new List<char> { ' ', '\n', '\t'};
        
        public static readonly Dictionary<LexicalTokensEnum, string> TypesToLexem = new Dictionary<LexicalTokensEnum, string>
        {
            { LexicalTokensEnum.Plus, "+" },
            { LexicalTokensEnum.Minus, "-" },
            { LexicalTokensEnum.Assignment, ":=" },
            { LexicalTokensEnum.Colon, ":" },
            { LexicalTokensEnum.Semicolon, ";" },
            { LexicalTokensEnum.Dot, "." },
            { LexicalTokensEnum.Comma, "," },
            { LexicalTokensEnum.ProgramKeyword, "program" },
            { LexicalTokensEnum.VarKeyword, "var" },
            { LexicalTokensEnum.BeginKeyword, "begin" },
            { LexicalTokensEnum.EndKeyword, "end" },
            { LexicalTokensEnum.Integer, "integer" },
            { LexicalTokensEnum.String, "string" },
        };
        
        public static readonly Dictionary<LexemType, string> LexemTypeToString = new Dictionary<LexemType, string>
        {
            { LexemType.Undefined, "Undefined" },
            { LexemType.Number, "number" },
            { LexemType.LexemSymbols, "term" },
            { LexemType.Identifier, "ident" },
            { LexemType.Keyword, "ident" }
        };
    }
}
