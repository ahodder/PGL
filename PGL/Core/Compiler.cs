using System.Diagnostics;
using PGL.Ast;
using PGL.Backend;
using PGL.Frontend;
using PGL.IL;

namespace PGL.Core;

public class Compiler
{
    private readonly ILogger _logger;
    private readonly Configuration _configuration;

    public Compiler(ILogger logger, Configuration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void Compile()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        if (_configuration.LogLevel >= ELogLevel.Info)
            _logger.Info(ECompilerStage.Startup, "PGL Performing Startup...");
        if (!Startup())
        {
            _logger.Error(ECompilerStage.Startup, "PGL Startup failed. Stopping...");
            return;
        }

        if (_configuration.LogLevel >= ELogLevel.Info)
            _logger.Info(ECompilerStage.Startup, "PGL Performing Lexing...");
        
        var allFileTokens = new List<List<Token>>();
        foreach (var sourceFile in _configuration.SourceFiles)
        {
            if (!PerformLexing(sourceFile, out var tokens))
            {
                _logger.Error(ECompilerStage.Startup, "PGL Lexing failed. Stopping...");
                return;
            }

            allFileTokens.Add(tokens);
        }
        
        if (_configuration.LogLevel >= ELogLevel.Info)
            _logger.Info(ECompilerStage.Parser, "PGL Performing Parsing...");

        var functions = new List<AstFunction>();
        foreach (var tokens in allFileTokens)
        {
            if (!PerformParsing(tokens, out var astFile))
            {
                _logger.Error(ECompilerStage.Parser, "Failed to parse file");
                goto CompilerEnd;
            }

            functions.AddRange(astFile.Functions);
        }

        var program = new AstProgram(_configuration, functions);
        
        if (_configuration.LogLevel >= ELogLevel.Info)
            _logger.Info(ECompilerStage.SemanticAnalysis, "PGL Performing semantic analysis...");

        if (!PerformSemanticAnalysis(program))
            _logger.Error(ECompilerStage.SemanticAnalysis, "Failed to finish semantic analysis");
        
        if (_configuration.LogLevel >= ELogLevel.Info)
            _logger.Info(ECompilerStage.IntermediateCodeGeneration, "PGL Performing intermediate code generation");
        
        if (!PerformIntermediateCodeGeneration(program, out var instructions))
            _logger.Error(ECompilerStage.IntermediateCodeGeneration, "Failed to perform intermediate code generation");
        
        if (_configuration.LogLevel >= ELogLevel.Info)
            _logger.Info(ECompilerStage.CompileTimeExecution, "PGL Performing compile time execution");
        
        if (!PerformCompileTimeExecution(instructions))
            _logger.Error(ECompilerStage.CompileTimeExecution, "Failed to perform compile time execution");

        CompilerEnd:
        stopwatch.Stop();
        _logger.Info(ECompilerStage.Shutdown, $"Compilation complete after: {stopwatch.Elapsed}");
    }

    /// <summary>
    /// Ensures that the parsed configuration is in a state that the compiler can consume it.
    /// </summary>
    /// <returns></returns>
    private bool Startup()
    {
        var dir = Directory.GetCurrentDirectory();
        var hasErrors = false;
        
        for (var i = 0; i < _configuration.SourceFiles.Count; i++)
        {
            var path = _configuration.SourceFiles[i];

            if (!Path.Exists(path))
            {
                var sourceFile = _configuration.SourceFiles[i];
                path = Path.Join(dir, sourceFile);
                if (!Path.Exists(path))
                {
                    _logger.Error(ECompilerStage.Lexer, $"Cannot lex file: {sourceFile}: file does not exist");
                    hasErrors = true;
                }
                else
                {
                    _configuration.SourceFiles[i] = path;
                }
            }
        }

        return !hasErrors;
    }

    private bool PerformLexing(string sourceFile, out List<Token> outTokens)
    {
        var fileContents = File.ReadAllText(sourceFile);

        var lexer = new Lexer(_logger, sourceFile, fileContents);
        var tokens = lexer.Tokenize();
        outTokens = tokens;

        return true;
    }

    private bool PerformParsing(List<Token> tokens, out AstFile ast)
    {
        var parser = new Parser(tokens);
        ast = parser.Parse();
        return true;
    }

    private bool PerformSemanticAnalysis(AstProgram ast)
    {
        var analyzer = new SemanticAnalysis(_configuration, ast);
        return analyzer.Analyze();
    }

    private bool PerformIntermediateCodeGeneration(AstProgram program, out List<ILInstruction> outInstructions)
    {
        var codeGenerator = new ILCodeGenerator(_configuration, program);
        codeGenerator.GenerateILCode();
        outInstructions = codeGenerator.Instructions;
        var str = codeGenerator.ToString();
        return true;
    }

    private bool PerformCompileTimeExecution(List<ILInstruction> instructions)
    {
        var vm = new VirtualMachine(_configuration, 96);
        vm.ExecuteProgram(instructions);
        Console.WriteLine(vm.PrintRegisters());
        return true;
    }

    private bool PerformTargetCodeGeneration()
    {
        return false;
    }

    private bool PerformLinking()
    {
        return false;
    }

    private bool PerformOutput()
    {
        return false;
    }

    private void Shutdown()
    {
    }
}