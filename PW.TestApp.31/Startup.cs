using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using PW.TestApp._31.Filters;
using PW.TestApp._31.Services;

//needed to use dependency injection
[assembly: FunctionsStartup(typeof(PW.TestApp._31.Startup))]

namespace PW.TestApp._31
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddTransient<IFunctionFilter, FunctionExceptionFilter>();
            builder.Services.AddScoped<IPetService, PetService>();
        }
    }
}
