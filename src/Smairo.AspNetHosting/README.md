# When to use Hosting extensions
When you need more than default configuration sources for aspnet core web applications. This library adds UserSecrets and Azure key vault as a configuration source and at the same time adds Serilog aspnetcore to log application from configuration

Also you can create Serilog logger using .net core default configuration sources when logger is required outside of web application startup (eg when running / creating web host)

## Example (.net web application)
Program.cs:
```
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Smairo.AspNetHosting;
namespace Smairo.Example.WebApplication.NetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = HostExtensions.CreateLogger();
            try
            {
                CreateWebHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception e)
            {
                logger.Fatal($"Host crashed... Reason: {e}");
            }
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                //.CreateDefaultBuilder(args) <-- Remove
                .CreateExtendedBuilderWithLogging<Startup>()
                .UseStartup<Startup>();
        }
    }
}
```