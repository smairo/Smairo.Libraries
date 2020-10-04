namespace Smairo.DependencyContainer.Azure
{
    /// <summary>
    /// Helpers for <see cref="IAzureFunctionHost"/>
    /// </summary>
    public static class AzureFunctionHostHelper
    {
        /// <summary>
        /// Initialize and get <see cref="IAzureFunctionHost"/>
        /// </summary>
        /// <returns></returns>
        public static IAzureFunctionHost GetFunctionHost()
        {
            return new AzureFunctionHost();
        }
    }
}