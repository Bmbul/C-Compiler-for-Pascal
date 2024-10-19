using Parser.Parser.Exceptions;

namespace Parser.Validators.ValidationExceptions
{
    public class OperationValidationException : ParsingException
    {
        public OperationValidationException(string explanation) : base($"Invalid operation exception: {explanation}")
        {
        
        }
    }
}