

using PGL.Core;

var configuration = new Configuration
{
    LogLevel = ELogLevel.Info,
    TargetPlatformInstructionSizeBytes = 8,
};

for (var i = 0; i < args.Length; i++)
{
    if (args[i].StartsWith('-'))
    {
        Console.WriteLine($"Handling flag: {args[i]}");
        // Handle flags
    }
    else
    {
        Console.WriteLine($"Found source file: {args[i]}");
        configuration.SourceFiles.Add(args[i]);
    }
}


var logger = new ConsoleLogger();
var compiler = new Compiler(logger, configuration);

try
{
    compiler.Compile();
}
catch (Exception e)
{
    Console.WriteLine($"The PGL compiler experienced an unexpected error and is now closing...\n{e}");
}
