using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace Smairo.DependencyContainer.Tests
{
    public class ContainerTests
    {
        // Container to create
        internal static readonly IServiceProvider Container =
            new ContainerBuilder()
                .RegisterModule(new TestStartup())
                .Build();

        [Fact]
        public void Should_Create_Container_And_Use_MyInjectableClass()
        {
            // Get service
            var testableClass = Container.GetService<IMyInjectableClass>();

            // Assert class
            Assert.True(testableClass.ShouldWork());
        }

        [Fact]
        public void Should_Create_Container_And_Use_Configuration_From_Json()
        {
            // Get configuration
            const string settingKey = "Key";
            const string expectedValue = "Value";
            var configuration = Container.GetService<IConfiguration>();
            var actualValue = configuration[settingKey];

            // Assert configuration
            Assert.Equal(expectedValue, actualValue);
        }
    }
}