namespace Neleus.DependencyInjection.Extensions
{
    /// <summary>
    /// Provides instances of registered services by name
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IServiceByNameFactory
    {
        /// <summary>
        /// Provides instance of registered service by name
        /// </summary>
        object GetByName(string name);
    }

    /// <summary>
    /// Provides instances of registered services by name
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IServiceByNameFactory<out TService> : IServiceByNameFactory
    {
        /// <summary>
        /// Provides instance of registered service by name
        /// </summary>
        new TService GetByName(string name);
    }
}