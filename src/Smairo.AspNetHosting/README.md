# When to use Hosting extensions
When you need more than default configuration sources for aspnet core web applications. This library adds UserSecrets and Azure key vault as a configuration source and at the same time adds Serilog aspnetcore to log application from configuration

Also you can create Serilog logger using .net core default configuration sources when logger is required outside of web application startup (eg when running / creating web host)

## Example (.net web application)
Program.cs:
```
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Smairo.AspNetHosting;
namespace Smairo.Example.WebApplication.NetCore
{
    // Install-Package Smairo.AspNetHosting
    public class Program
    {
        public static void Main(string[] args)
        {
            using var logger = HostExtensions.CreateLogger(new string[0]);
            try
            {
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception e)
            {
                logger.Fatal("Aspnet host crashed! ", e);
                Console.WriteLine(e);
            }
            finally
            {
                logger.Dispose();
            }
        }

        // If nothing special required
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .CreateExtendedBuilderWithSerilog<Startup>(args);

        // If customization required
        public static IHostBuilder CreateHostBuilder2(string[] args) =>
            new HostBuilder()
                .CreateExtendedBuilderWithSerilog<Startup>(
                    args, 
                    kestrelConfiguration =>
                    {
                        kestrelConfiguration.Limits.MaxConcurrentConnections = 5000;
                    },
                    customConfiguration =>
                    {
                        customConfiguration.AddJsonFile("mycustomfile.json", optional: true);
                    });
    }
}
```