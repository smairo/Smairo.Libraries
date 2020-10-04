using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
namespace Smairo.DependencyContainer.Azure
{
    /// <summary>
    /// Build DI collection from Function startup for debugging function from another solution
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class FunctionDebuggerBuilder<TStartup>
        where TStartup : AzureFunctionStartup<TStartup>
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// ctor
        /// </summary>
        public FunctionDebuggerBuilder()
        {
            IFunctionsHostBuilder functionHost = new FunctionDebuggerHost();
            var startupInstance = Activator.CreateInstance<TStartup>();
            startupInstance.Configure(functionHost);
            _provider = functionHost.Services.BuildServiceProvider();
        }

        /// <summary>
        /// Wrapper for getting the service from <see cref="IServiceProvider"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService GetService<TService>()
        {
            return _provider.GetService<TService>();
        }
    }
}