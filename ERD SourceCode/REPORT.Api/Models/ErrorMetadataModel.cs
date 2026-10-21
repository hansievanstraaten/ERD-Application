namespace REPORT.Api.Models
{
    public class ErrorMetadataModel
    {
        public string ExceptionKey { get; set; } = string.Empty;

        public Dictionary<string, object>? MetadataValues { get; set; }
    }
}
