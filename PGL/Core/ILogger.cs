namespace PGL.Core;

public enum ELogLevel
{
    None,
    Error,
    Warning,
    Info,
}

public enum ECompilerStage
{
    Startup,
    Lexer,
    Parser,
    SemanticAnalysis,
    IntermediateCodeGeneration,
    CompileTimeExecution,
    TargetCodeGeneration,
    Linking,
    Output,
    Shutdown,
}

public interface ILogger
{
    void Info(ECompilerStage stage, string message);
    void Warning(ECompilerStage stage, string message);
    void Error(ECompilerStage stage, string message);
}

public class ConsoleLogger : ILogger
{
    public void Info(ECompilerStage stage, string message)
    {
        Console.WriteLine($"[INFO-{stage}] {message}");
    }

    public void Warning(ECompilerStage stage, string message)
    {
        Console.WriteLine($"[WARNING-{stage}] {message}");
    }

    public void Error(ECompilerStage stage, string message)
    {
        Console.WriteLine($"[ERROR-{stage}] {message}");
    }
}

public static class LoggerExtensions
{
    public static void LexerError(this ILogger self, string sourceFile, uint line, uint column, string message)
    {
        self.Error(ECompilerStage.Lexer, $"[{sourceFile}-{line}:{column}]\n{message}");
    }
}