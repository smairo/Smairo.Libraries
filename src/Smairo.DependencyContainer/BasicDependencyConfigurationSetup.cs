using Microsoft.Extensions.Configuration;
namespace Smairo.DependencyContainer
{
    /// <summary>
    /// Basic setup for <see cref="IConfigurationBuilder"/>
    /// </summary>
    public static class BasicDependencyConfigurationSetup
    {
        /// <summary>
        /// Create basic configuration: json files, environmental variables and azure key vault if key vault file (as json) provided
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="jsonFilesToAdd"></param>
        /// <param name="keyVaultFile"></param>
        /// <returns></returns>
        public static IConfiguration CreateBasicConfigurations(this IConfigurationBuilder configurationBuilder, string[] jsonFilesToAdd, string keyVaultFile = null)
        {
            // Determine if injection with azure functions. Has no effect on other types
            var functionHost = AzureFunctionHostHelper.GetFunctionHost();
            configurationBuilder
                .SetBasePath(functionHost.GetHostRootPath());

            foreach (var file in jsonFilesToAdd)
            {
                configurationBuilder
                    .AddJsonFile(file, optional: true);
            }

            if (!string.IsNullOrWhiteSpace(keyVaultFile))
            {
                configurationBuilder
                    .AddJsonFile(keyVaultFile);
            }

            var configuration = configurationBuilder
                .AddEnvironmentVariables()
                .Build();

            const string vaultSectionName = "KeyVault";
            if (configuration.GetSection(vaultSectionName).Exists())
            {
                configurationBuilder.AddAzureKeyVault(
                    vault: configuration.GetSection(vaultSectionName)["Vault"],
                    clientId: configuration.GetSection(vaultSectionName)["ClientId"],
                    clientSecret: configuration.GetSection(vaultSectionName)["ClientSecret"]
                );
            }

            return configurationBuilder.Build();
        }
    }
}