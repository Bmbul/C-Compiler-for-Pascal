using System.Collections.Generic;
using Common;
using Common.Utility;
using Compiler.Generator.CodeGenerator;
using Parser.Parser.Exceptions;
using Parser.Parser.ParserStateMachine;
using Parser.Validators.ValidationExceptions;
using Parser.Validators;

namespace Parser.Parser
{
    public class Parser : IParser, IParsingResultModifier
    {
        private readonly LexicalAnalyserResult _tokens;
        private readonly IStateMachine _parsingStateMachine;
        private ParsingResult _parsingResult;
        private IAssemblerGenerator _assemblerGenerator;
        private IOperationValidator _operationValidator;
        private readonly string integerKeyword = Constants.TypesToLexem[LexicalTokensEnum.Integer];


        public Parser(LexicalAnalyserResult tokens)
        {
            _tokens = tokens;
            _parsingStateMachine = new StateMachine(this);
            _parsingResult = new ParsingResult();
            _operationValidator = new OperationValidator();
        }
        
        public void Parse(string outputFileName)
        {
            while (_tokens.TryGetNextToken(out var token))
            {
                _parsingStateMachine.ProcessTokens(token);
            }

            _assemblerGenerator.GetGeneratedCode($"{outputFileName}.s");
        }
        
        public LexicalToken GetNextToken()
        {
            if (_tokens.TryGetNextToken(out var lexicalToken))
            {
                return lexicalToken;
            }
            throw new ParsingException("Unexpected end of the program.");
        }

        public bool TryGetNextToken(out LexicalToken lexicalToken)
        {
            return _tokens.TryGetNextToken(out lexicalToken);
        }

        public bool IsValidVariableToken(LexicalToken token)
        {
            return token.IsIdentifier() && _parsingResult.Variables.ContainsKey(token.Value);
        }

        public void ValidateAssignment(LexicalToken variableToken, LexicalToken assigneeToken)
        {
			if (assigneeToken.Type == LexemType.Identifier)
			{
				ValidateVariableIsDefined(assigneeToken.Value);
			}
            var assigneeType = assigneeToken.IsNumber() ? integerKeyword : _parsingResult.Variables[assigneeToken.Value].Type;
            ValidateAssignment(variableToken, assigneeType);
        }

		private void ValidateVariableIsDefined(string variableName)
		{
            if (!_parsingResult.IsAssigned(variableName))
            {
                throw new OperationValidationException($"Use of unassigned variable '{variableName}'.");
            }
		}

        public void ValidateAssignment(LexicalToken variableToken, string assigneeType)
        {
            var resultType = variableToken.IsNumber() ? integerKeyword : _parsingResult.Variables[variableToken.Value].Type;
            _operationValidator.ValidateAssignment(resultType, assigneeType);
        }

        public void ValidateOperation(LexicalToken first, LexicalToken second, LexicalToken operation, out string resultType)
        {
            if (first.Type == LexemType.Identifier)
            {
                ValidateVariableIsDefined(first.Value);
            }
            if (second.Type == LexemType.Identifier)
            {
                ValidateVariableIsDefined(second.Value);
            }
            
            var firstType = first.IsNumber() ? integerKeyword : _parsingResult.Variables[first.Value].Type;
            var secondType = second.IsNumber() ? integerKeyword : _parsingResult.Variables[second.Value].Type;

            resultType = _operationValidator.ValidateOperation(firstType, secondType, operation);
        }
        
        public bool IdentifierExists(string variableValue)
        {
            return _parsingResult.Variables.ContainsKey(variableValue);
        }

        public void SetProgramName(string programName)
        {
            if (_parsingResult.ProgramName != null)
            {
                throw new ParsingException("Program name is already declared");
            }
            _parsingResult.ProgramName = programName;
            _assemblerGenerator = new AssemblerGenerator(programName);
        }

        public void AddVariables(List<string> identifiers, string dataType)
        {
            _parsingResult.DeclareVariables(identifiers, dataType);
        }

        public void DeclareVariables()
        {
            var variables = _parsingResult.GetVariables();
            _assemblerGenerator.DeclareVariables(variables);
        }

        public void GenerateSimpleAssignment(LexicalToken token, LexicalToken firstVariable)
        {
            ValidateAssignment(token, firstVariable); // validate a := b
            
            var destination = $"{_parsingResult.ProgramName}_{token.Value}";
            var source = GetVariableAssemblerNaming(firstVariable);
            _assemblerGenerator.GenerateSimpleAssignment(destination, source);
            _parsingResult.SetAssigned(token.Value);
        }

        public void GenerateAssignmentWithOperations(LexicalToken token, LexicalToken firstVariable, LexicalToken secondVariable,
            LexicalToken operation)
        {
            ValidateOperation(firstVariable, secondVariable, operation,
                out var resultingType); // validate b [operator] c (out resultType)

            if (firstVariable.IsNumber() && secondVariable.IsNumber())
            {
                OptimizedAssignmentForNumbers(token, firstVariable, secondVariable, operation);
            }
            else
            {
                ValidateAssignment(token, resultingType); // validate a := result of (b [operator] c)
                var destination = $"{_parsingResult.ProgramName}_{token.Value}";
                var firstVar = GetVariableAssemblerNaming(firstVariable);
                var secondVar = GetVariableAssemblerNaming(secondVariable);
                _assemblerGenerator.GenerateAssignmentAfterOperation(destination, firstVar, secondVar, operation.Value);
            	_parsingResult.SetAssigned(token.Value);
	    }
        }

        public void GenerateExitWithLastOperationResult()
        {
            _assemblerGenerator.GenerateExitWithLastOperationResult();
        }

        private string GetVariableAssemblerNaming(LexicalToken token)
        {
            return token.IsNumber() ? $"${token.Value}" : $"{_parsingResult.ProgramName}_{token.Value}";
        }
        
        private void OptimizedAssignmentForNumbers(LexicalToken token, LexicalToken firstVariable, LexicalToken secondVariable,
            LexicalToken operation)
        {
            var firstNum = long.Parse(firstVariable.Value);
            var secondNum = long.Parse(secondVariable.Value);
            if (operation.IsSpecialToken(Constants.TypesToLexem[LexicalTokensEnum.Plus]))
            {
                firstVariable = new IntToken(firstNum + secondNum);
            }
            else
            {
                firstVariable = new IntToken(firstNum - secondNum);
            }
            GenerateSimpleAssignment(token, firstVariable);
        }

        public void AddStatementsSection()
        {
            _assemblerGenerator.AddGlobalAndTextSection();
        }
    }
}
