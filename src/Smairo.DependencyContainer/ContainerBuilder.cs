using System;
using Microsoft.Extensions.DependencyInjection;
namespace Smairo.DependencyContainer
{
    /// <inheritdoc />
    /// <summary>
    /// This represents the builder entity for IoC container.
    /// </summary>
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerBuilder"/> class.
        /// </summary>
        public ContainerBuilder()
        {
            _services = new ServiceCollection();
        }

        /// <inheritdoc />
        public IContainerBuilder RegisterModule(IModule module = null)
        {
            if (module == null)
            {
                module = new Module();
            }

            module.Load(_services);
            return this;
        }

        /// <inheritdoc />
        public IServiceProvider Build()
        {
            return _services
                .BuildServiceProvider();
        }
    }
}