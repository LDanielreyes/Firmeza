namespace Firmeza.Models;

public class ImportResult
{
    public string Message { get; set; } = "Processing started.";
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
    public List<string> Log { get; set; } = new List<string>();
    
    public bool Success => !Errors.Any();

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Status: {(Success ? "SUCCESS" : "FAILED")}");
        sb.AppendLine($"Message: {Message}");
        if (Errors.Any()) sb.AppendLine($"Errors ({Errors.Count}):\n\t{string.Join("\n\t", Errors)}");
        if (Warnings.Any()) sb.AppendLine($"Warnings ({Warnings.Count}):\n\t{string.Join("\n\t", Warnings)}");
        if (Log.Any()) sb.AppendLine($"Log ({Log.Count}):\n\t{string.Join("\n\t", Log)}");
        return sb.ToString();
    }
}