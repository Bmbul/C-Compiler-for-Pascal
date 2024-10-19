using LexicalAnalyser.Analyser;
using System;
using System.IO;
using Common;
using Parser.Parser;
using Common.ProcessRunner;

public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length != 1)
            {
                throw new ApplicationException("Wrong number of arguments.");
            }
            ILexicalAnalyser analyser = new BetterLexicalAnalyser(args[0]);
            var tokens = analyser.Tokenize();
            
            if (tokens.ContainsInvalid(out LexicalToken wrongToken))
            {
                throw new ApplicationException($"Bad token: {wrongToken.Value}");
            }
            
            IParser parser = new Parser.Parser.Parser(tokens);
            var fileNameWithoutExtention = Path.GetFileNameWithoutExtension(args[0]);
            parser.Parse(fileNameWithoutExtention);

            ICommandRunner commandRunner = new CommandRunner("gcc", $"-nostartfiles -no-pie -o {fileNameWithoutExtention} {fileNameWithoutExtention}.s");
            commandRunner.RunCommand();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

