namespace Smairo.DependencyContainer.Tests
{
    public class MyInjectableClass : IMyInjectableClass
    {
        /// <inheritdoc />
        public bool ShouldWork()
        {
            return true;
        }
    }

    public interface IMyInjectableClass
    {
        bool ShouldWork();
    }
}