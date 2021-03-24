using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserAvatar.Api.Extentions
{
    /// <summary>
    /// Middleware to log raw data
    /// </summary>
    internal class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public LoggingMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
        }

        /// <summary>
        /// Invoke method of logging.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var requestInformation = $"Request information:\n" +
                                     $"Schema:{context.Request.Scheme}\n" +
                                     $"Host:{context.Request.Host}\n" +
                                     $"Path:{context.Request.Path}\n" +
                                     $"QueryString:{context.Request.QueryString}\n" +
                                     $"Request Body:{await ObtainRequestBody(context.Request)}\n";
            _logger.LogInformation(requestInformation);

            var originalResponseBody = context.Response.Body;
            await using var responseBody = new MemoryStream();
            
            context.Response.Body = responseBody;
            await _next(context);

            var status = GetStatusCode(context);
            var level = GetLogLevel(status);

            _logger.Log(level, "Response body: LogLevel: {0}; Code: {1}\n Body: {2}",
                GetLogLevel(status),status,await ObtainResponseBody(context));

            await responseBody.CopyToAsync(originalResponseBody);

        }

        private static async Task<string> ObtainRequestBody(HttpRequest request)
        {
            if (request.Body == null) return string.Empty;
            request.EnableBuffering();
            var encoding = GetEncodingFromContentType(request.ContentType);
            string bodyStr;

            using (var reader = new StreamReader(request.Body, encoding, true, 1024, true))
            {
                bodyStr = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            request.Body.Seek(0, SeekOrigin.Begin);
            return bodyStr;
        }
        private static async Task<string> ObtainResponseBody(HttpContext context)
        {
            var response = context.Response;
            response.Body.Seek(0, SeekOrigin.Begin);
            var encoding = GetEncodingFromContentType(response.ContentType);
            using var reader = new StreamReader(response.Body, encoding, detectEncodingFromByteOrderMarks:
                false, bufferSize: 4096, leaveOpen: true);
            var text = await reader.ReadToEndAsync().ConfigureAwait(false);
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
        private static Encoding GetEncodingFromContentType(string contentTypeStr)
        {
            if (string.IsNullOrEmpty(contentTypeStr))
            {
                return Encoding.UTF8;
            }
            ContentType contentType;
            try
            {
                contentType = new ContentType(contentTypeStr);
            }
            catch (FormatException)
            {
                return Encoding.UTF8;
            }
            if (string.IsNullOrEmpty(contentType.CharSet))
            {
                return Encoding.UTF8;
            }
            return Encoding.GetEncoding(contentType.CharSet, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
        }
        private static LogLevel GetLogLevel(int? statusCode)
        {
            return statusCode > 399 ? LogLevel.Error : LogLevel.Information;
        }
        private static int? GetStatusCode(HttpContext context)
        {
            return context.Response?.StatusCode;
        }
        
    }
}