using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
namespace Smairo.AspNetHosting
{
    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="IHostBuilder"/> with azure key vault and serilog
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Create <see cref="Serilog.Core.Logger"/> using base configuration sources (app settings, environmental variables)
        /// </summary>
        /// <returns></returns>
        public static Serilog.Core.Logger CreateLogger(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .CreateBaseConfigurations(args)
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Logger = logger;

            return logger;
        }

        /// <summary>
        /// Create more robust host with configuration sources (app settings, env specific app settings, environmental variables, user secrets and azure key vault) + serilog logging
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <returns></returns>
        public static IHostBuilder CreateExtendedBuilderWithSerilog<TStartup>(
            this IHostBuilder hostBuilder,
            string[] args,
            Action<IConfigurationBuilder> customAppConfiguration = null) 
            where TStartup : class
        {
            hostBuilder
                .ConfigureWebHostDefaults(host =>
                {
                    host.UseStartup<TStartup>();
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(appConfiguration => CreateAppConfiguration<TStartup>(appConfiguration, args))
                .UseSerilog(CreateSerilogLogging);

            if (customAppConfiguration != null)
            {
                hostBuilder
                    .ConfigureAppConfiguration(customAppConfiguration);
            }

            return hostBuilder;
        }

        /// <summary>
        /// Serilog.Aspnetcore logging
        /// </summary>
        /// <param name="hostContext"></param>
        /// <param name="loggerConfiguration"></param>
        internal static void CreateSerilogLogging(HostBuilderContext hostContext, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostContext.Configuration);
        }

        /// <summary>
        /// Create basic and advanced configuration
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="appConfiguration"></param>
        /// <param name="args"></param>
        private static void CreateAppConfiguration<TStartup>(IConfigurationBuilder appConfiguration, string[] args)
            where TStartup : class
        {
            appConfiguration
                .CreateBaseConfigurations(args)
                .CreateAdvancedConfigurations<TStartup>();
        }

        /// <summary>
        /// Basic: Commandline, appsettings, appsettings.env, environmentalVariables
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static IConfigurationBuilder CreateBaseConfigurations(this IConfigurationBuilder configurationBuilder, string[] args)
        {
            var environmentString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            return configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentString}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }


        /// <summary>
        /// Advanced: User secrets, azure key vault. Override is possible with user secrets
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="configurationBuilder"></param>
        /// <returns></returns>
        internal static IConfigurationBuilder CreateAdvancedConfigurations<TStartup>(this IConfigurationBuilder configurationBuilder)
            where TStartup : class
        {
            configurationBuilder
                .AddJsonFile("keyvault.json", optional: true, reloadOnChange: true)
                .TryAddUserSecrets<TStartup>();

            IConfiguration configuration = configurationBuilder
                .Build();

            configurationBuilder
                .TryAddAzureKeyVault(configuration)
                // Add again so that you can override key vault values
                .TryAddUserSecrets<TStartup>() 
                .Build();

            return configurationBuilder;
        }

        /// <summary>
        /// Tries to add user secrets
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="configurationBuilder"></param>
        /// <returns></returns>
        internal static IConfigurationBuilder TryAddUserSecrets<TStartup>(this IConfigurationBuilder configurationBuilder)
            where TStartup : class
        {
            using (var logger = CreateLogger(new string[0]))
            {
                try
                {
                    configurationBuilder
                        .AddUserSecrets<TStartup>();
                }
                catch (Exception e)
                {
                    const string error = "Exception occured when trying to add UserSecrets to app configuration. " + 
                                         "If your app does not support user secrets, this was expected.";
                    Console.WriteLine($"[{DateTime.UtcNow:s}] [Warning] {error}");
                    logger.Warning(error, e);
                }
            }
            return configurationBuilder;
        }

        /// <summary>
        /// Tries to add azure key vault using given configuration
        /// </summary>
        /// <returns></returns>
        internal static IConfigurationBuilder TryAddAzureKeyVault(this IConfigurationBuilder configurationBuilder, IConfiguration configuration)
        {
            var vaultSection = configuration?.GetSection("AzureKeyVault");
            var vaultUrl = vaultSection?["VaultUrl"];
            var clientId = vaultSection?["ClientId"];
            var clientSecret = vaultSection?["ClientSecret"];

            using (var logger = CreateLogger(new string[0]))
            {
                if (VaultSettingsHasValues(vaultUrl, clientId, clientSecret))
                {
                    configurationBuilder.AddAzureKeyVault(vaultUrl, clientId, clientSecret);
                    logger.Information("Valid Azure KeyVault configuration. Added to configuration sources");
                }
                else
                {
                    const string error = "Azure key vault could not be added. This might indicate that AzureKeyVault section " +
                                         "(or AzureKeyVault:ClientId, AzureKeyVault:ClientSecret) is missing from configuration";
                    Console.WriteLine($"[{DateTime.UtcNow:s}] [Warning] {error}");
                    logger.Warning(error);
                }
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
    }
}