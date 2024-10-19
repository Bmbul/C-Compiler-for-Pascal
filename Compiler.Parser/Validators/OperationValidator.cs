using Common;
using Common.Utility;
using Parser.Validators.ValidationExceptions;

namespace Parser.Validators
{
    public class OperationValidator : IOperationValidator
    {
        private readonly string plusKeyword = Constants.TypesToLexem[LexicalTokensEnum.Plus];
        private readonly string minusKeyword = Constants.TypesToLexem[LexicalTokensEnum.Minus];
        private readonly string stringKeyword = Constants.TypesToLexem[LexicalTokensEnum.String];
        private readonly string integerKeyword = Constants.TypesToLexem[LexicalTokensEnum.Integer];
        public void ValidateAssignment(string resultType, string assigneeType)
        {
            if (resultType != assigneeType)
            {
                throw new OperationValidationException($"Type mismatch: typeof({resultType}) != typeof({assigneeType}).");
            }
        }

        public string ValidateOperation(string firstType, string secondType, LexicalToken operation)
        {
            string resultType = null;
            if (operation.IsSpecialToken(minusKeyword))
            {
                if (firstType == secondType && firstType == integerKeyword)
                {
                    resultType = integerKeyword;
                }
            }
            else if (operation.IsSpecialToken(plusKeyword))
            {
                if (firstType == secondType)
                {
                    resultType = firstType;
                }
                else if ((firstType == integerKeyword && secondType == stringKeyword) 
                         || (firstType == integerKeyword && secondType == stringKeyword))
                {
                    resultType = stringKeyword;
                }
            }
            
            if (resultType == null)
            {
                throw new OperationValidationException( $"Type mismatch: {firstType} {operation.Value} {secondType}");
            }
            return resultType;
        }
    }
}