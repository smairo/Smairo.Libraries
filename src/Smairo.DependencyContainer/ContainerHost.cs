using Microsoft.Extensions.DependencyInjection;

namespace Smairo.DependencyContainer
{
    /// <summary>
    /// "Host" to provide <see cref="IServiceCollection"/>
    /// </summary>
    public interface IContainerHost
    {
        /// <summary>
        /// <see cref="IServiceCollection"/> for the "Host"
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <inheritdoc />
    public class ContainerHost : IContainerHost
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ContainerHost()
        {
            Services = new ServiceCollection();
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }
}
