using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PW.TestApp._31.Filters
{
    internal class AuthorizeAttribute : FunctionInvocationFilterAttribute
    {
        public AuthorizeAttribute() 
        {
        }

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {

            if (executingContext.Arguments.TryGetValue("req", out object request))
            {
                var httpContext = ((HttpRequest)request)?.HttpContext;

                if (httpContext == null || !httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    throw new InvalidOperationException();
                }
                else if (httpContext.Request.Headers["Authorization"] != "123456")
                {
                    throw new UnauthorizedAccessException();
                }
            }

            await base.OnExecutingAsync(executingContext, cancellationToken);
        }
    }
}
