using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Neleus.DependencyInjection.Extensions
{
    /// <summary>
    /// Provides easy fluent methods for building named registrations of the same interface
    /// </summary>
    public class ServicesByNameBuilder
    {
        internal readonly Type _serviceType;

        internal readonly ServiceContainer _services;

        internal readonly IDictionary<string, Type> _registrations;

        internal ServicesByNameBuilder(Type serviceType, ServiceContainer services, NameBuilderSettings settings)
        {
            _serviceType = serviceType;
            _services = services;
            _registrations = settings.CaseInsensitiveNames
                ? new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, Type>();
        }

        /// <summary>
        /// Maps name to corresponding implementation.
        /// Note that this implementation has to be also registered in IoC container so
        /// that <see cref="IServiceByNameFactory&lt;TService&gt;"/> is be able to resolve it.
        /// </summary>
        public ServicesByNameBuilder Add(string name, Type implementationType)
        {
            if (!this._serviceType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException($"Provided implementation does not implement {this._serviceType.FullName}");
            }

            _registrations.Add(name, implementationType);
            return this;
        }

        /// <summary>
        /// Generic version of <see cref="Add"/>
        /// </summary>
        public ServicesByNameBuilder Add<TImplementation>(string name)
        {
            return this.Add(name, typeof(TImplementation));
        }

        /// <summary>
        /// Adds <see cref="IServiceByNameFactory&lt;TService&gt;"/> to IoC container together with all registered implementations
        /// so it can be consumed by client code later. Note that each implementation has to be also registered in IoC container so
        /// <see cref="IServiceByNameFactory&lt;TService&gt;"/> is be able to resolve it from the container.
        /// </summary>
        public virtual void Build()
        {
            IDictionary<string, Type> registrations = _registrations;
            //Registrations are shared across all instances
            _services.AddService(
                typeof(IServiceByNameFactory<>).MakeGenericType(this._serviceType),
                (s, t) => Activator.CreateInstance(typeof(ServiceByNameFactory<>).MakeGenericType(this._serviceType), s, registrations)
            );
        }
    }

    /// <summary>
    /// Provides easy fluent methods for building named registrations of the same interface
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class ServicesByNameBuilder<TService> : ServicesByNameBuilder
    {
        internal ServicesByNameBuilder(ServiceContainer services, NameBuilderSettings settings) 
            : base(typeof(TService), services, settings)
        {
        }

        /// <summary>
        /// Maps name to corresponding implementation.
        /// Note that this implementation has to be also registered in IoC container so
        /// that <see cref="IServiceByNameFactory&lt;TService&gt;"/> is be able to resolve it.
        /// </summary>
        public ServicesByNameBuilder<TService> Add(string name, Type implementationType)
        {
            _registrations.Add(name, implementationType);
            return this;
        }

        /// <summary>
        /// Generic version of <see cref="Add"/>
        /// </summary>
        public ServicesByNameBuilder<TService> Add<TImplementation>(string name) where TImplementation : TService
        {
            return Add(name, typeof(TImplementation));
        }

        /// <summary>
        /// Adds <see cref="IServiceByNameFactory&lt;TService&gt;"/> to IoC container together with all registered implementations
        /// so it can be consumed by client code later. Note that each implementation has to be also registered in IoC container so
        /// <see cref="IServiceByNameFactory&lt;TService&gt;"/> is be able to resolve it from the container.
        /// </summary>
        public override void Build()
        {
            IDictionary<string, Type> registrations = _registrations;
            //Registrations are shared across all instances
            _services.AddService(typeof(IServiceByNameFactory<TService>), (s, t) => new ServiceByNameFactory<TService>(s, registrations));
        }
    }
}