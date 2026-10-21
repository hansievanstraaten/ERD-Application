using REPORT.Api.Models;
using System.Net;

namespace REPORT.Api.Custom
{
    public class ApiException : ApplicationException
    {
        public HttpStatusCode StatusCode { get; }

        public ErrorMetadataModel? MetaData { get; set; }

        public ApiException(HttpStatusCode statusCode, string message, ErrorMetadataModel? metaData = null) : base(message)
        {
            this.StatusCode = statusCode;
            this.MetaData = metaData;
        }
    }
}
