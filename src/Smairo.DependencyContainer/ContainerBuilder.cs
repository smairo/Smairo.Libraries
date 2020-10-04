using System;
using Microsoft.Extensions.DependencyInjection;
namespace Smairo.DependencyContainer
{
    /// <summary>
    /// Wraps <see cref="IServiceProvider"/> for given <typeparam name="TStartup">TStartup</typeparam>
    /// </summary>
    public class ContainerBuilder<TStartup>
       where TStartup : BaseStartup 
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// ctor
        /// </summary>
        public ContainerBuilder()
        {
            IContainerHost containerHost = new ContainerHost();
            var startupInstance = Activator.CreateInstance<TStartup>();
            startupInstance.Configure(containerHost);
            _provider = containerHost
                .Services
                .BuildServiceProvider();
        }

        /// <summary>
        /// Get service from the wrapper
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService GetService<TService>()
        {
            return _provider.GetService<TService>();
        }
    }
}