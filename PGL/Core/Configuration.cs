namespace PGL.Core;

public class Configuration
{
    public ELogLevel LogLevel { get; set; }

    public List<string> SourceFiles { get; } = new List<string>();
    
    public byte TargetPlatformInstructionSizeBytes { get; set; }
}