# When to use Hosting extensions
When you need more than default configuration sources for aspnet core web applications. This library adds UserSecrets and Azure key vault as a configuration source and at the same time adds Serilog aspnetcore to log application from configuration.

You can then inherit ApiStartup for your startup class to create preconfigured, clean and structured setup.

Also you can create Serilog logger using .net core default configuration sources when logger is required outside of web application startup (eg when running / creating web host)

## Example (.net web application)
Program.cs:
```csharp
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
                    customConfiguration =>
                    {
                        customConfiguration.AddJsonFile("mycustomfile.json", optional: true);
                    });
    }
}
```

Startup.cs
```
public class Startup : ApiStartup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        : base(configuration, environment)
    {
    }

    public override OpenApiInfo ApiInfo => new OpenApiInfo
    {
        Title = "My api",
        Version = "v1"
    };

    public override void AddAndConfigureOptions(IServiceCollection services)
    {
        services.AddOptions();
        services.Configure<MyConfigurableOptions>(opt =>
        {
            opt.ServiceLevel = 1;
            opt.RetryCount = 0;
        });
    }

    public override void AddAuthenticationAndAuthorization(IServiceCollection services)
    {
        services.AddJwtAuthentication();
        services.AddAuthorization(auth =>
        {
            auth.AddPolicy("MyAuthPolicy", builder => { });
        });
    }

    public override void AddDatabase(IServiceCollection services)
    {
        services.AddDbContext<Model.MyDbContext>();
    }

    public override void AddOurServices(IServiceCollection services)
    {
        services.AddScoped<IMyService, MyService>();
    }

    public override string ApiDocumentationXmlPath => Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
}
```