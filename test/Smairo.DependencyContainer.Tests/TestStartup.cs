using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Smairo.DependencyContainer.Tests
{
    public class TestStartup : BaseStartup
    {
        public override IConfiguration SetupConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testSettings.json", optional: false)
                .Build();

            return configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMyInjectableClass, MyInjectableClass>();
            services.AddTransient(_ => Configuration);
        }
    }
}