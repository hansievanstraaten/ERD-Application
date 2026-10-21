using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using REPORT.Api.Constants;
using REPORT.Api.Custom;
using REPORT.Api.ExtensionMothods;
using REPORT.Api.Models;
using System.Net;

namespace REPORT.Api.Middleware
{
    public class ValidationMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleWare> _logger;

        public ValidationMiddleWare(RequestDelegate next, ILogger<ValidationMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IConfiguration config)
        {
            try
            {
                var path = context.Request.Path;

                // Allow Swagger UI and its supporting files without an API key.
                if (context.Request.Path == "/" || 
                    path.StartsWithSegments("/swagger"))
                {
                    await _next(context);
                    return;
                }

                if (!context.Request.Headers.TryGetValue(AppConstants.X_API_KEY, out StringValues extractedApiKey)
                || StringValues.IsNullOrEmpty(extractedApiKey))
                {
                    throw new ApiException(HttpStatusCode.Unauthorized, "API Key is missing or not provided in the request headers.");
                }

                string configKey = config[$"Authentication:{AppConstants.X_API_KEY}"]!;

                if (extractedApiKey.Equals(configKey) == false)
                {
                    throw new ApiException(HttpStatusCode.Unauthorized, "Invalid API Key provided in the request headers.");
                }

                await _next(context);
            }
            catch (ApiException err)
            {
                _logger.LogWarning(err, "Handled ApiException");
                await HandleExceptionAsync(context, err);
            }
            catch (Exception err)
            {
                _logger.LogError(err, "Unhandled exception occurred");
                await HandleExceptionAsync(context, err);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, ApiException exception)
        {
            HttpStatusCode code = exception.StatusCode;

            ErrorResponseModel response = new ErrorResponseModel();
            response.SetError(exception.MessageFull(), exception.StackTraceFull(), exception.MetaData);

            string? result = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected

            ErrorResponseModel response = new ErrorResponseModel();

            response.SetError(exception.MessageFull(), exception.StackTraceFull());

            string? result = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
