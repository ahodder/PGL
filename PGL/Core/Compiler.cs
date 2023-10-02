using PGL.Frontend;

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

        foreach (var tokens in allFileTokens)
        {
            PerformParsing(tokens);
        }
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

    private bool PerformParsing(List<Token> tokens)
    {
        var parser = new Parser(tokens);
        var ast = parser.Parse();
        return true;
    }

    private bool PerformSemanticAnalysis()
    {
        return false;
    }

    private bool PerformIntermediateCodeGeneration()
    {
        return false;
    }

    private bool PerformCompileTimeExecution()
    {
        return false;
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