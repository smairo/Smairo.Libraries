namespace Smairo.DependencyContainer.Azure
{
    /// <summary>
    /// Convenient way to get information about where function is running and what is the path where the functions runs
    /// </summary>
    public interface IAzureFunctionHost
    {
        /// <summary>
        /// Get root of application files
        /// </summary>
        /// <returns></returns>
        string GetHostRootPath();

        /// <summary>
        /// Check if we are running now in the Azure service
        /// </summary>
        /// <returns></returns>
        bool RunsCurrentlyInAzure();

        /// <summary>
        /// Check if we are running now in the local computer
        /// </summary>
        /// <returns></returns>
        bool RunsCurrentlyInLocal();

        /// <summary>
        /// Get environment as string, eg 'Development' from the global configuration
        /// </summary>
        /// <returns></returns>
        string GetEnvironment();
    }
}