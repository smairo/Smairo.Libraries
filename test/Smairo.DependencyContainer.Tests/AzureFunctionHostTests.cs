using System.IO;
using Smairo.DependencyContainer.Azure;
using Xunit;
namespace Smairo.DependencyContainer.Tests
{
    public class AzureFunctionHostTests
    {
        [Fact]
        public void Is_Running_Locally()
        {
            var azFunctionHost = AzureFunctionHostHelper.GetFunctionHost();
            Assert.True(azFunctionHost.RunsCurrentlyInLocal());
            Assert.False(azFunctionHost.RunsCurrentlyInAzure());
            Assert.Equal(Directory.GetCurrentDirectory(), azFunctionHost.GetHostRootPath());
        }
    }
}