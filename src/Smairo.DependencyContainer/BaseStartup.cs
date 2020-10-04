using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smairo.DependencyContainer.Azure;

namespace Smairo.DependencyContainer
{
    /// <summary>
    /// Inherit to get startup type of setup for Dependency Injection to any type of project.
    /// For azure functions, inherit <see cref="AzureFunctionStartup{TStartup}"/> instead
    /// </summary>
    public abstract class BaseStartup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        protected IConfiguration Configuration;

        /// <summary>
        /// Create configurations and configure services
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(IContainerHost builder)
        {
            Configuration = SetupConfiguration();
            ConfigureServices(builder.Services);
        }

        /// <summary>
        /// Your setup for configurations (eg json files)
        /// </summary>
        /// <returns></returns>
        public abstract IConfiguration SetupConfiguration();

        /// <summary>
        /// Your setup for IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigureServices(IServiceCollection services);
    }
}