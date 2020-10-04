using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
namespace Smairo.DependencyContainer.Azure
{
    /// <summary>
    /// Wrapper for <see cref="IServiceCollection"/> that enables function debugging from another assembly
    /// </summary>
    internal class FunctionDebuggerHost : IFunctionsHostBuilder
    {
        /// <summary>
        /// <see cref="IServiceCollection"/>
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public FunctionDebuggerHost()
        {
            Services = new ServiceCollection();
        }
    }
}