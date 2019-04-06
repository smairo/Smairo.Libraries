using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Smairo.DependencyContainer.Tests
{
    public class TestStartup : IModule
    {
        /// <summary>
        /// Add all required dependencies to <see cref="IServiceCollection"/> for tests
        /// </summary>
        /// <param name="services"></param>
        public void Load(IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testSettings.json", optional: false)
                .Build();

            services.AddTransient<IMyInjectableClass, MyInjectableClass>();
            services.AddTransient(_ => configuration);
        }
    }
}