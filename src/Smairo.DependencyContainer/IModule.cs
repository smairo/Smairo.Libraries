using Microsoft.Extensions.DependencyInjection;
namespace Smairo.DependencyContainer
{
    /// <summary>
    /// This provides interfaces to the <see cref="Module"/> class.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Loads dependencies to the collection.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance.</param>
        void Load(IServiceCollection services);
    }
}