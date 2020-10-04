using System;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Smairo.DependencyContainer.Azure
{
    /// <summary>
    /// Create DI container for Azure functions (https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
    /// <br/> Adds configurations (jsons, env, keyvault, user secrets) and basic service collection setup
    /// <br/> TStartup should be your startup class
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public abstract class AzureFunctionStartup<TStartup> : FunctionsStartup
    {
        /// <summary>
        /// Configuration from the container
        /// </summary>
        protected IConfiguration Configuration;

        /// <summary>
        /// Function environment
        /// </summary>
        protected IAzureFunctionHost Environment;

        /// <summary>
        /// Func spec. Adds to builder.Services
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Configuration = CreateAndBuildConfiguration();

            // Add IConfiguration as singleton to service collection
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));

            // Add IOptions support
            builder.Services.AddOptions();

            // Add HttpClientFactory support
            builder.Services.AddHttpClient();

            // Run users configurations for the service collection
            ConfigureServices(builder.Services);
        }

        /// <summary>
        /// All your required services and setup should be done here
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Overridable configuration creator. Adds Json files (appsettings, dev specific appsettings, localsettings), env, keyvault and user secrets
        /// </summary>
        /// <returns></returns>
        public virtual IConfiguration CreateAndBuildConfiguration()
        {
            Environment = AzureFunctionHostHelper.GetFunctionHost();
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Environment.GetHostRootPath());

            configBuilder
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("keyvault.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironment()}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            TryAddKeyvaultAndUserSecrets(configBuilder);

            return configBuilder
                .Build();
        }

        private static void TryAddKeyvaultAndUserSecrets(IConfigurationBuilder configBuilder)
        {
            var userSecretAssembly = Assembly.GetAssembly(typeof(TStartup));

            // Add user secrets
            try
            {
                configBuilder.AddUserSecrets(userSecretAssembly);
            }
            catch
            {
                const string error = "Exception occured when trying to add UserSecrets to app configuration. " + 
                    "If your app does not support user secrets, this was expected.";
                Console.WriteLine($"[{DateTime.UtcNow:s}] [Warning] {error}");
            }

            // Add keyvault
            var temporaryConfiguration = configBuilder
                .Build();
            AddKeyvault(temporaryConfiguration, configBuilder);

            // Add user secrets again to allow keyvault overwrite
            try
            {
                configBuilder.AddUserSecrets(userSecretAssembly);
            }
            catch
            {
                const string error = "Exception occured when trying to add UserSecrets to app configuration. " + 
                    "If your app does not support user secrets, this was expected.";
                Console.WriteLine($"[{DateTime.UtcNow:s}] [Warning] {error}");
            }
        }

        private static void AddKeyvault(IConfiguration temporaryConfiguration, IConfigurationBuilder configBuilder)
        {
            var vaultSection = temporaryConfiguration?.GetSection("AzureKeyVault");
            var vaultUrl = vaultSection?["VaultUrl"];
            var clientId = vaultSection?["ClientId"];
            var clientSecret = vaultSection?["ClientSecret"];

            if (VaultSettingsHasValues(vaultUrl, clientId, clientSecret))
            {
                configBuilder.AddAzureKeyVault(vaultUrl, clientId, clientSecret);
                Console.WriteLine($"[{DateTime.UtcNow:s}] [Information] Valid Azure KeyVault configuration. Added to configuration sources");
            }
            else
            {
                const string error = "Azure key vault could not be added. This might indicate that AzureKeyVault section " +
                    "(or AzureKeyVault:ClientId, AzureKeyVault:ClientSecret) is missing from configuration";
                Console.WriteLine($"[{DateTime.UtcNow:s}] [Warning] {error}");
            }
        }

        /// <summary>
        /// Validate required vault parameters
        /// </summary>
        /// <param name="vaultUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        internal static bool VaultSettingsHasValues(string vaultUrl, string clientId, string clientSecret)
        {
            return !string.IsNullOrWhiteSpace(vaultUrl)
                && !string.IsNullOrWhiteSpace(clientId)
                && !string.IsNullOrWhiteSpace(clientSecret);
        }
    }
}
