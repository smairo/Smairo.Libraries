# How to use dependency container
You create a starup class that inherits the IModule. Then add your services etc to the startup class and then create containerBuilder where ever you wish to access DI container.

## Example (.net console project)
Starup.cs:
```
// Create startup class and inherit IModule from Smairo.DependencyContainer
public class Startup : IModule
{
	// Add all services and stuff to IServiceCollection that you wish to use
	public void Load(IServiceCollection services)
	{
		services.AddOptions();
		services.Configure<SomeOptions>(opt =>
		{
			opt.Setting1 = "Setting";
			opt.Setting2 = "OtherSetting";
		});
		services.AddScoped<IMyClass, MyClass>();
	}
}
```

Program.cs:
```
public class Program
{
	// Create container using Startup class as a module, then build it to static variable
	public static IServiceProvider Provider = new ContainerBuilder()
												.RegisterModule(new Startup())
												.Build();
	static void Main(string[] args)
	{
		// Get IMyClass from container
		var myDiClass = Provider.GetService<IMyClass>();

		// Use IMyClass
		myDiClass.DoSomething();
	}
}
```

You can use ContainerBuilder to add dependency injection to pretty much anywhere (eg adding dependency injection to Azure Functions V2, making WPF app completely using DI etc.)