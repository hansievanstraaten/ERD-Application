namespace REPORT.Api.Models
{
    public class ErrorResponseModel
    {
        public bool HaveError { get; set; }

        public string? ErrorMessage { get; set; }

        public string? StackTrace { get; set; }

        public ErrorMetadataModel? ErrorMetadata { get; set; }

        public void SetError(string message, string stackTrace, ErrorMetadataModel? errorData = null)
        {
            this.HaveError = true;
            this.ErrorMessage = message;
            this.ErrorMetadata = errorData;
            this.StackTrace = stackTrace;
        }
    }
}
