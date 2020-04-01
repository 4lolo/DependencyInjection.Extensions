﻿using System;
using System.Collections.Generic;

namespace Neleus.DependencyInjection.Extensions
{
    internal class ServiceByNameFactory<TService> : IServiceByNameFactory<TService>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _registrations;

        public ServiceByNameFactory(IServiceProvider serviceProvider, IDictionary<string, Type> registrations)
        {
            _serviceProvider = serviceProvider;
            _registrations = registrations;
        }

        object IServiceByNameFactory.GetByName(string name)
        {
            if (!_registrations.TryGetValue(name, out Type implementationType))
            {
                throw new ArgumentException($"Service name '{name}' is not registered");
            }

            return (TService)_serviceProvider.GetService(implementationType);
        }

        public TService GetByName(string name)
        {
            if (!_registrations.TryGetValue(name, out Type implementationType))
            {
                throw new ArgumentException($"Service name '{name}' is not registered");
            }

            return (TService)_serviceProvider.GetService(implementationType);
        }
    }
}
