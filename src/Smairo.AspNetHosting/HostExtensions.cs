using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
namespace Smairo.AspNetHosting
{
    public static class HostExtensions
    {
        /// <summary>
        /// Create <see cref="Serilog.Core.Logger"/> using base configuration sources (app settings, environmental variables)
        /// </summary>
        /// <returns></returns>
        public static Serilog.Core.Logger CreateLogger()
        {
            var configuration = new ConfigurationBuilder()
                .CreateBaseConfiguration()
                .Build();

            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        /// <summary>
        /// Create more robust host with configuration sources (app settings, env specific app settings, environmental variables, user secrets and azure key vault) + serilog logging
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <returns></returns>
        public static IWebHostBuilder CreateExtendedBuilderWithLogging<TStartup>() where TStartup : class
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(CreateConfigurations<TStartup>)
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureServices(services =>
                {
                    services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
                })
                .UseSerilog(CreateSerilogLogging);
        }

        #region Internals
        /// <summary>
        /// Base configuration
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        internal static IConfigurationBuilder CreateBaseConfiguration(this IConfigurationBuilder builder)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            return builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        /// <summary>
        /// Base + User Secrets with error handling + Key vault with error handling
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="hostContext"></param>
        /// <param name="configurationBuilder"></param>
        internal static void CreateConfigurations<TStartup>(WebHostBuilderContext hostContext, IConfigurationBuilder configurationBuilder) where TStartup : class
        {
            configurationBuilder
                .CreateBaseConfiguration()
                .AddJsonFile("keyvault.json", optional: true, reloadOnChange: true)
                .TryAddUserSecrets<TStartup>();

            // Build so that we can try to get key vault values from configurations
            var configuration = configurationBuilder
                .Build();

            // Key vault
            configurationBuilder
                .TryAddAzureKeyVault(configuration)
                .Build();
        }

        /// <summary>
        /// Serilog.Aspnetcore logging
        /// </summary>
        /// <param name="hostContext"></param>
        /// <param name="loggerConfiguration"></param>
        internal static void CreateSerilogLogging(WebHostBuilderContext hostContext, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostContext.Configuration);
        }

        /// <summary>
        /// Tires to add user secrets
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="configurationBuilder"></param>
        /// <returns></returns>
        internal static IConfigurationBuilder TryAddUserSecrets<TStartup>(this IConfigurationBuilder configurationBuilder) where TStartup : class
        {
            try
            {
                configurationBuilder
                    .AddUserSecrets<TStartup>();
            }
            catch (Exception e)
            {
                // Should be shown in Kudu and app logs
                Console.WriteLine($"Exception occured when trying to add UserSecrets to app configuration. If your app does not support user secrets, this was expected. Exception: {e}");
            }
            return configurationBuilder;
        }

        /// <summary>
        /// Tries to add azure key vault using given configuration
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IConfigurationBuilder TryAddAzureKeyVault(this IConfigurationBuilder configurationBuilder, IConfiguration configuration)
        {
            var vaultSection = configuration?.GetSection("AzureKeyVault");
            var vaultUrl = vaultSection?["VaultUrl"];
            var clientId = vaultSection?["ClientId"];
            var clientSecret = vaultSection?["ClientSecret"];

            var logger = CreateLogger();

            if (VaultSettingsHasValues(vaultUrl, clientId, clientSecret))
            {
                configurationBuilder.AddAzureKeyVault(vaultUrl, clientId, clientSecret);
                logger.Information("Valid Azure KeyVault configuration. Added to configuration sources");
            }
            else
            {
                // Should be shown in Kudu and app logs
                Console.WriteLine($"Azure key vault could not be added. This might indicate that AzureKeyVault section is missing from configuration");
                logger.Warning("Invalid Azure KeyVault configuration. KeyVault won't be used!");
            }
            return configurationBuilder;
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
        #endregion
    }
}
