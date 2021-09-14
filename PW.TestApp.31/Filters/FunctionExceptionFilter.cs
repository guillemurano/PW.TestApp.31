using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PW.TestApp._31.Filters
{
    public class FunctionExceptionFilter : IFunctionExceptionFilter
    {
        private readonly ILogger<FunctionExceptionFilter> _log;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FunctionExceptionFilter(IHttpContextAccessor httpContextAccessor, ILogger<FunctionExceptionFilter> log)
        {
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }

        public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            _log.LogError($"Exception: { exceptionContext.Exception?.Message }");
            _log.LogError($"Inner Exception: { exceptionContext.Exception.InnerException?.Message ?? "No inner exception" }");

            switch (exceptionContext.Exception.InnerException)
            {
                case UnauthorizedAccessException ex:
                    return Result(StatusCodes.Status401Unauthorized, ErrorMessage.Unauthorized);
                case InvalidOperationException ex:
                    return Result(StatusCodes.Status400BadRequest, ErrorMessage.BadRequest);
                case Exception ex:
                    return Result(StatusCodes.Status500InternalServerError, ErrorMessage.InternalServerError);
            }

            return Task.CompletedTask;

            Task Result(int status, string message)
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = status;
                return _httpContextAccessor.HttpContext.Response.WriteAsync(message);
            }
        }
    }
}
