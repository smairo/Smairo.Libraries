# How to use dependency container
You create a startup class that inherits the IModule. Then add your services etc to the startup class and then create containerBuilder where ever you wish to access DI container.

## Example (.net console project and other non azure functions)
Starup.cs:
```csharp
// Install-Package Smairo.DependencyContainer

// Create startup class and inherit BaseStartup from Smairo.DependencyContainer
public class Startup : BaseStartup
{
	// Create your configurations
	public override IConfiguration SetupConfiguration()
	{
		IConfiguration configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("testSettings.json", optional: false)
			.Build();

		return configuration;
	}

	// Setup your services to collection
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<IMyClass, MyClass>();
	}
}
```

Program.cs:
```csharp
public class Program
{
	// Create container 
	public static readonly ContainerBuilder<Startup> Container =
            new ContainerBuilder<Startup>();
		
	static void Main(string[] args)
	{
		// Get IMyClass from container
		var myDiClass = Container.GetService<IMyClass>();

		// Use IMyClass
		myDiClass.DoSomething();
	}
}
```

You can use ContainerBuilder to add dependency injection to pretty much anywhere else also (eg Wpf).

## Example (Azure functions)
Azure functions has extended version (AzureFunctionStartup) within lib (in Smairo.DependencyContainer.Azure namespace). Library creates a default setup for configuration that uses:
- local.settings.json
- appsettings.json
- appsettings.{env}.json
- keyvault.json
- Environmental variables
- User secrets (within TStartup assembly)

If those are not enough, you should override CreateAndBuildConfiguration().
Environment is controlled with 'ASPNETCORE_ENVIRONMENT' environmental variable.

Startup.cs
```csharp
[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]
namespace MyNamespace
{
    public class Startup : AzureFunctionStartup<Startup>
    {
        public override IConfiguration CreateAndBuildConfiguration()
        {
            return base.CreateAndBuildConfiguration();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMyClass, MyClass>();
        }
    }
}
```

Function1.cs
```csharp
namespace MyNamespace
{
    public class Function1
    {
        public readonly IMyClass _myClass;
        public Function1(IMyClass myClass)
        {
            _myClass = myClass;
        }

        [FunctionName(nameof(Function1))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(_myClass.GetHelloWorld());
        }
    }
}
```